using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using OpenIddict.Server;

namespace ReflectieveDuck.McpServer.Auth;

/// <summary>
/// Voegt dynamische ChatGPT/Claude redirect URIs toe aan de client VOORDAT
/// OpenIddict de standaard redirect_uri validatie uitvoert.
/// Zo hoeven we niet elke connector-ID handmatig te registreren.
/// </summary>
public sealed class DynamicRedirectUriHandler : IOpenIddictServerHandler<OpenIddictServerEvents.ValidateAuthorizationRequestContext>
{
    private static readonly string[] TrustedPrefixes =
    [
        "https://chatgpt.com/connector/",
        "https://chatgpt.com/aip/",
        "https://chat.openai.com/aip/",
        "https://claude.ai/api/mcp/"
    ];

    private readonly IOpenIddictApplicationManager _manager;

    public DynamicRedirectUriHandler(IOpenIddictApplicationManager manager)
        => _manager = manager;

    public async ValueTask HandleAsync(OpenIddictServerEvents.ValidateAuthorizationRequestContext context)
    {
        if (string.IsNullOrEmpty(context.ClientId) || string.IsNullOrEmpty(context.RedirectUri))
            return;

        // Check of de redirect URI matcht met een vertrouwd prefix
        var matchesPrefix = false;
        foreach (var prefix in TrustedPrefixes)
        {
            if (context.RedirectUri.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                matchesPrefix = true;
                break;
            }
        }

        if (!matchesPrefix)
            return;

        // Zoek de client en voeg de redirect URI toe als die nog niet geregistreerd is
        var application = await _manager.FindByClientIdAsync(context.ClientId);
        if (application is null)
            return;

        var redirectUris = await _manager.GetRedirectUrisAsync(application);

        if (!redirectUris.Contains(context.RedirectUri))
        {
            var descriptor = new OpenIddictApplicationDescriptor();
            await _manager.PopulateAsync(descriptor, application);
            descriptor.RedirectUris.Add(new Uri(context.RedirectUri));
            await _manager.UpdateAsync(application, descriptor);
        }
    }
}
