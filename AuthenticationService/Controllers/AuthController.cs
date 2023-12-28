using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;
using AuthenticationService.Core.Extensions;

namespace AuthenticationService.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet, HttpPost, Route("auth")]
    public async Task<IActionResult> AuthAsync()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ??
        throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        // Retrieve the user principal stored in the authentication cookie.
        //var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var result = await HttpContext.AuthenticateAsync("cookie");

        // If the user principal can't be extracted, redirect the user to the login page.
        if (!result.Succeeded)
        {
            return Challenge(
                authenticationSchemes: "cookie",
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                });
        }

        // Create a new claims principal
        var sub = new Guid(result.Principal.GetClaim(Claims.Subject)!);
        var claims = new List<Claim>
    {
        // 'subject' claim which is required
        new (Claims.Subject, sub.ToString() ?? string.Empty),
        new (Claims.Name, result.Principal.GetClaim(Claims.Name) ?? string.Empty),
        new ("sub-b64", sub.ToBase64Url() ?? string.Empty),
    };

        var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Set requested scopes (this is not done automatically)
        claimsPrincipal.SetScopes(request.GetScopes());
        claimsPrincipal.SetDestinations(static claim => claim.Type switch
        {
            _ => new[]
            {
                Destinations.AccessToken,
                Destinations.IdentityToken,
            }
        });

        // Signing in with the OpenIddict authentiction scheme trigger OpenIddict to issue a code (which can be exchanged for an access token)
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    }
}
