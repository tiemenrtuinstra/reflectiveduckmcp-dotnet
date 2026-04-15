using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace ReflectieveDuck.McpServer.Auth;

/// <summary>
/// Dynamic Client Registration (DCR) per RFC 7591.
/// Claude.ai en ChatGPT registreren zichzelf als OAuth client via dit endpoint.
/// Beveiligd met een registratie-token (DCR_TOKEN environment variable).
/// </summary>
[ApiController]
[Route("connect")]
public class DcrController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IConfiguration _config;
    private readonly ILogger<DcrController> _logger;

    public DcrController(
        IOpenIddictApplicationManager applicationManager,
        IConfiguration config,
        ILogger<DcrController> logger)
    {
        _applicationManager = applicationManager;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// POST /connect/register — Registreer een nieuwe OAuth client (DCR).
    /// Vereist een Bearer token in de Authorization header (DCR_TOKEN).
    /// Retourneert client_id en client_secret per RFC 7591.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] DcrRequest request)
    {
        // CRIT-2 fix: DCR endpoint beveiligen met registratie-token
        var expectedToken = _config["DCR_TOKEN"]
            ?? Environment.GetEnvironmentVariable("DCR_TOKEN");

        if (!string.IsNullOrEmpty(expectedToken))
        {
            var authHeader = Request.Headers.Authorization.ToString();
            if (!authHeader.Equals($"Bearer {expectedToken}", StringComparison.Ordinal))
            {
                _logger.LogWarning("DCR: Ongeautoriseerde registratie-poging van {IP}",
                    HttpContext.Connection.RemoteIpAddress);
                return Unauthorized(new { error = "invalid_token", error_description = "Geldig DCR registratie-token vereist." });
            }
        }

        // Genereer een unieke client_id en secret
        var clientId = $"dcr-{Guid.NewGuid():N}"[..24];
        var clientSecret = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();

        var redirectUris = request.RedirectUris ?? [];

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            DisplayName = request.ClientName ?? "MCP Client",
            ClientType = OpenIddictConstants.ClientTypes.Confidential,
            ConsentType = OpenIddictConstants.ConsentTypes.Implicit
        };

        foreach (var uri in redirectUris)
        {
            if (Uri.TryCreate(uri, UriKind.Absolute, out var parsed))
                descriptor.RedirectUris.Add(parsed);
        }

        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
        descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);

        foreach (var scope in request.Scope?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? ["mcp"])
        {
            descriptor.Permissions.Add($"{OpenIddictConstants.Permissions.Prefixes.Scope}{scope}");
        }

        await _applicationManager.CreateAsync(descriptor);

        _logger.LogInformation("DCR: Client geregistreerd — {ClientId} ({ClientName})", clientId, descriptor.DisplayName);

        return Ok(new DcrResponse
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            ClientName = descriptor.DisplayName,
            RedirectUris = redirectUris,
            GrantTypes = ["authorization_code", "refresh_token"],
            ResponseTypes = ["code"],
            TokenEndpointAuthMethod = "client_secret_post",
            ClientIdIssuedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ClientSecretExpiresAt = 0
        });
    }
}

public record DcrRequest
{
    [JsonPropertyName("client_name")] public string? ClientName { get; init; }
    [JsonPropertyName("redirect_uris")] public string[]? RedirectUris { get; init; }
    [JsonPropertyName("scope")] public string? Scope { get; init; }
    [JsonPropertyName("grant_types")] public string[]? GrantTypes { get; init; }
    [JsonPropertyName("response_types")] public string[]? ResponseTypes { get; init; }
    [JsonPropertyName("token_endpoint_auth_method")] public string? TokenEndpointAuthMethod { get; init; }
}

public record DcrResponse
{
    [JsonPropertyName("client_id")] public required string ClientId { get; init; }
    [JsonPropertyName("client_secret")] public required string ClientSecret { get; init; }
    [JsonPropertyName("client_name")] public string? ClientName { get; init; }
    [JsonPropertyName("redirect_uris")] public string[]? RedirectUris { get; init; }
    [JsonPropertyName("grant_types")] public string[]? GrantTypes { get; init; }
    [JsonPropertyName("response_types")] public string[]? ResponseTypes { get; init; }
    [JsonPropertyName("token_endpoint_auth_method")] public string? TokenEndpointAuthMethod { get; init; }
    [JsonPropertyName("client_id_issued_at")] public long ClientIdIssuedAt { get; init; }
    [JsonPropertyName("client_secret_expires_at")] public long ClientSecretExpiresAt { get; init; }
}
