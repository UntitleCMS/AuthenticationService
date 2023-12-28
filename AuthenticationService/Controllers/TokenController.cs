using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

namespace AuthenticationService.Controllers;

[ApiController]
public class TokenController : ControllerBase
{
    [HttpGet, HttpPost, Route("/token")]
    public async Task<IActionResult> ExchangeAsync()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        ClaimsPrincipal claimsPrincipal;

        if (request.IsClientCredentialsGrantType())
        {
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());
            identity.AddClaim("some-claim", "some-value", OpenIddictConstants.Destinations.AccessToken);
            claimsPrincipal = new ClaimsPrincipal(identity);
            claimsPrincipal.SetScopes(request.GetScopes());
        }

        else if (request.IsAuthorizationCodeGrantType())
        {
            // Retrieve the claims principal stored in the authorization code
            claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
        }

        else if (request.IsRefreshTokenGrantType())
        {
            // Retrieve the claims principal stored in the refresh token.
            claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
        }

        else
        {
            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    }
}
