using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ReflectieveDuck.McpServer.Auth;

/// <summary>
/// Handelt de OAuth 2.0 Authorization Code + PKCE flow af.
/// Claude.ai, ChatGPT en andere clients worden hier naartoe geredirect
/// om een authorization code op te halen.
/// </summary>
[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;

    public AuthorizationController(IOpenIddictApplicationManager applicationManager)
        => _applicationManager = applicationManager;

    /// <summary>
    /// GET /authorize — OAuth authorize endpoint.
    /// Auto-approvet de aanvraag (geen consent pagina nodig voor persoonlijk gebruik).
    /// </summary>
    [HttpGet("~/authorize"), HttpPost("~/authorize")]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("De OAuth request kon niet worden opgehaald.");

        // Haal de client applicatie op
        var application = await _applicationManager.FindByClientIdAsync(request.ClientId!)
            ?? throw new InvalidOperationException($"Client '{request.ClientId}' niet gevonden.");

        var displayName = await _applicationManager.GetDisplayNameAsync(application) ?? request.ClientId;

        // Bouw een ClaimsIdentity voor de authorization code
        var identity = new ClaimsIdentity(
            authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            nameType: Claims.Name,
            roleType: Claims.Role);

        // Voeg claims toe die in de tokens terechtkomen
        identity.AddClaim(Claims.Subject, request.ClientId!);
        identity.AddClaim(Claims.Name, displayName!);

        // Stel de scopes in die zijn aangevraagd (of alle beschikbare)
        identity.SetScopes(request.GetScopes());
        identity.SetDestinations(static claim => claim.Type switch
        {
            Claims.Name => [Destinations.AccessToken, Destinations.IdentityToken],
            _ => [Destinations.AccessToken]
        });

        // Geef de authorization code terug (redirect naar client)
        return SignIn(new ClaimsPrincipal(identity),
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// POST /token — OAuth token endpoint.
    /// Wisselt een authorization code of refresh token in voor access tokens.
    /// Dit wordt automatisch afgehandeld door OpenIddict.
    /// </summary>
    [HttpPost("~/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("De OAuth token request kon niet worden opgehaald.");

        if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
        {
            // Haal de principal op uit de authorization code of refresh token
            var result = await HttpContext.AuthenticateAsync(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            if (result?.Principal is null)
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var identity = (ClaimsIdentity)result.Principal.Identity!;
            identity.SetDestinations(static claim => claim.Type switch
            {
                Claims.Name => [Destinations.AccessToken, Destinations.IdentityToken],
                _ => [Destinations.AccessToken]
            });

            return SignIn(result.Principal,
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return BadRequest(new OpenIddictResponse
        {
            Error = Errors.UnsupportedGrantType,
            ErrorDescription = "Alleen authorization_code en refresh_token grants zijn ondersteund."
        });
    }
}
