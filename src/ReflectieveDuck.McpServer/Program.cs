using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using ReflectieveDuck.Extensions;
using ReflectieveDuck.McpServer.Prompts;
using ReflectieveDuck.McpServer.Tools;
using ReflectieveDuck.Shared.Infrastructure.LocalDb;

var builder = WebApplication.CreateBuilder(args);

// ── Sentry — error monitoring ───────────────────────────────────────────────
builder.WebHost.UseSentry(o =>
{
    o.Dsn = Environment.GetEnvironmentVariable("SENTRY_DSN")
        ?? "https://6be89257f7b27d2c56783b9c57386e39@o4511228687417344.ingest.de.sentry.io/4511228714877008";
    o.TracesSampleRate = 0.2; // 20% van requests voor performance monitoring
    o.SendDefaultPii = false; // Geen persoonlijke data naar Sentry
    o.Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "production";
    o.Release = "reflectieve-duck@1.0.0";
});

// Fly.io TLS termination: forward het originele HTTPS scheme
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// ── Transport detectie ──────────────────────────────────────────────────────
var transport = args.FirstOrDefault(a => a.StartsWith("--transport="))
    ?.Split('=')[1]
    ?? Environment.GetEnvironmentVariable("MCP_TRANSPORT")
    ?? "stdio";

// In stdio mode: geen Kestrel URLs binden
if (transport != "http")
    builder.WebHost.UseUrls();

// ── ReflectieveDuck services ────────────────────────────────────────────────
builder.Services.AddReflectieveDuck(builder.Configuration);

// ── CORS — nodig voor browser-based OAuth flows (Claude.ai, ChatGPT) ────────
builder.Services.AddCors(opts => opts.AddDefaultPolicy(policy =>
    policy.WithOrigins(
            "https://claude.ai",
            "https://chat.openai.com",
            "https://chatgpt.com")
        .AllowAnyHeader()
        .AllowAnyMethod()));

// ── Data Protection — persistente keys op /data volume ──────────────────────
var dataPath = Environment.GetEnvironmentVariable("DB_PATH") is { } dbp
    ? Path.GetDirectoryName(dbp) ?? "/data"
    : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ReflectieveDuck");

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(dataPath, "dp-keys")))
    .SetApplicationName("reflectieve-duck");

// ── OpenIddict — embedded OAuth Authorization Server ────────────────────────
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<LocalDbContext>();
    })
    .AddServer(options =>
    {
        // OAuth 2.0 Authorization Code + PKCE flow
        options.AllowAuthorizationCodeFlow()
            .RequireProofKeyForCodeExchange();

        // Refresh tokens voor langere sessies
        options.AllowRefreshTokenFlow();

        // Endpoint URIs
        options.SetAuthorizationEndpointUris("/authorize");
        options.SetTokenEndpointUris("/token");

        // Scopes registreren (HIGH-4 fix)
        options.RegisterScopes("mcp");

        // Token configuratie
        options.SetAccessTokenLifetime(TimeSpan.FromHours(1));
        options.SetRefreshTokenLifetime(TimeSpan.FromDays(14));

        // ASP.NET Core integration
        options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough();

        // CRIT-1 fix: Data Protection voor persistente token encryptie/signing
        // Ephemeral keys zijn nog nodig voor JWKS metadata endpoint
        options.UseDataProtection();
        options.AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey();

        // Sta onversleutelde token requests toe (achter Fly.io TLS terminator)
        if (transport == "http")
            options.UseAspNetCore().DisableTransportSecurityRequirement();
    })
    .AddValidation(options =>
    {
        // Valideer tokens die door deze server zijn uitgegeven
        options.UseLocalServer();
        options.UseDataProtection();
        options.UseAspNetCore();
    });

// ── Authentication & Authorization ──────────────────────────────────────────
builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

// Controllers voor OAuth endpoints (AuthorizationController, DcrController)
builder.Services.AddControllers();

// ── MCP Server ──────────────────────────────────────────────────────────────
var mcpBuilder = builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "reflectieve-duck",
            Version = "1.0.0"
        };
    })
    // ── Tools (45 stuks) ──────────────────────────────────────────────────
    .WithTools<StoplichtTools>()       // 4 tools: status, geschiedenis, update, vergelijk
    .WithTools<FeedbackTools>()        // 5 tools: toevoegen, lijst, zoeken, statistieken, tags
    .WithTools<ReflectieTools>()       // 1 tool:  reflectievragen genereren
    .WithTools<AnalyseTools>()         // 2 tools: code analyse, stacktrace analyse
    .WithTools<ContextTools>()         // 4 tools: patronen, inzichten, volledig, dashboard
    .WithTools<ResourceTools>()        // 8 tools: ijsberg, addendum, asswijzer, reflectie-assistent, lifemap, sterktes, health, config
    .WithTools<ProductiviteitTools>()  // 4 tools: focus start/stop, energie log, analyse
    .WithTools<WellbeingTools>()       // 9 tools: tips, codewoord, kernkwadrant, emmer_*, rolverdeling, volledig
    .WithTools<ReflectieModuleTools>() // 9 tools: ToM, EF, sociale coherentie, dating, duck-modus, retrospective, kernkwadrant_analyse, ijsberg_analyse
    // ── Prompts (15 stuks) ────────────────────────────────────────────────
    .WithPrompts<WellbeingPrompts>()   // 6 prompts: emmer-check, reflectiesessie, check-in, SCARF, weekoverzicht, diepe analyse
    .WithPrompts<AgentPrompts>();      // 9 agents: wellbeing coach, debug duck, retro facilitator, sociale navigator, energie manager, feedback coach, autisme coach, werk coach, prikkel adviseur

if (transport == "http")
{
    mcpBuilder.WithHttpTransport();
    mcpBuilder.AddAuthorizationFilters();
}
else
{
    mcpBuilder.WithStdioServerTransport();
}

// ── Logging ─────────────────────────────────────────────────────────────────
builder.Logging.AddConsole(options =>
    options.LogToStandardErrorThreshold = LogLevel.Trace);

var app = builder.Build();

// ── Database initialisatie ──────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LocalDbContext>();
    db.Database.EnsureCreated();

    // HIGH-3 fix: WAL mode voor veilige writes bij Fly.io auto_stop
    db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
}

// ── HTTP pipeline ───────────────────────────────────────────────────────────
if (transport == "http")
{
    app.UseForwardedHeaders();
    app.UseSentryTracing();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers(); // OAuth endpoints: /authorize, /token, /connect/register

    // ── Publieke endpoints ──────────────────────────────────────────────────

    app.MapGet("/", () => Results.Content("""
        <!DOCTYPE html>
        <html lang="nl">
        <head><meta charset="utf-8"><title>Reflectieve Duck MCP</title>
        <style>
            body { font-family: system-ui, sans-serif; max-width: 600px; margin: 60px auto; padding: 0 20px; color: #1a1a1a; }
            h1 { color: #2d5016; } a { color: #2d7d46; } code { background: #f0f0f0; padding: 2px 6px; border-radius: 3px; }
            .tools { columns: 2; } .tools li { break-inside: avoid; }
        </style></head>
        <body>
            <h1>&#x1F986; Reflectieve Duck MCP Server</h1>
            <p>Welzijns-MCP voor neurodivergente developers.</p>
            <p><strong>MCP endpoint:</strong> <code>/mcp</code> (OAuth beveiligd)</p>
            <p><strong>OAuth metadata:</strong>
                <a href="/.well-known/openid-configuration">/.well-known/openid-configuration</a></p>
            <p><strong>DCR:</strong> <code>POST /connect/register</code></p>
            <h3>45 tools + 15 prompts/agents</h3>
            <ul class="tools">
                <li>stoplicht_status</li><li>stoplicht_geschiedenis</li>
                <li>stoplicht_update</li><li>stoplicht_vergelijk</li>
                <li>stoplicht_tips</li><li>stoplicht_codewoord</li>
                <li>stoplicht_rolverdeling</li><li>stoplicht_volledig</li>
                <li>feedback_toevoegen</li><li>feedback_lijst</li>
                <li>feedback_zoeken</li><li>feedback_statistieken</li>
                <li>feedback_tags</li><li>reflectie_vragen</li>
                <li>code_analyse</li><li>stacktrace_analyse</li>
                <li>context_patronen</li><li>context_inzichten</li>
                <li>context_volledig</li><li>dashboard</li>
                <li>resource_ijsberg</li><li>resource_addendum</li>
                <li>resource_asswijzer</li><li>resource_reflectie_assistent</li>
                <li>resource_lifemap</li><li>resource_sterktes</li>
                <li>resource_health</li><li>resource_config</li>
                <li>focus_start</li><li>focus_stop</li>
                <li>energie_log</li><li>productiviteit_analyse</li>
                <li>kernkwadrant</li><li>emmer_strategieen</li>
                <li>emmer_anderen</li><li>emmer_triggers</li>
                <li>emmer_reactie</li>
                <li>module_theory_of_mind</li><li>module_executieve_functies</li>
                <li>module_sociale_coherentie</li><li>module_dating</li>
                <li>module_reflectieve_duck</li><li>module_retrospective</li>
                <li>module_kernkwadrant_analyse</li>
                <li>module_ijsberg_analyse</li>
            </ul>
        </body></html>
        """, "text/html"));

    app.MapGet("/health", () => Results.Json(new
    {
        status = "ok",
        transport = "http",
        version = "1.0.0"
    }));

    app.MapGet("/server-info", () => Results.Json(new
    {
        name = "reflectieve-duck",
        version = "1.0.0",
        transport = "http",
        mcpEndpoint = "/mcp",
        oauthMetadata = "/.well-known/openid-configuration",
        dcr = "/connect/register",
        tools = 45,
        prompts = 15
    }));

    // MED-1 fix: MCP OAuth Protected Resource metadata (RFC 9728)
    app.MapGet("/.well-known/oauth-protected-resource", (HttpContext ctx) => Results.Json(new
    {
        resource = $"{ctx.Request.Scheme}://{ctx.Request.Host}",
        authorization_servers = new[] { $"{ctx.Request.Scheme}://{ctx.Request.Host}" },
        scopes_supported = new[] { "mcp" },
        bearer_methods_supported = new[] { "header" },
        resource_name = "Reflectieve Duck MCP Server"
    }));

    // ── MCP endpoint (beveiligd — HIGH-5 fix) ──────────────────────────────
    app.MapMcp("/mcp").RequireAuthorization();
}

app.Run();
