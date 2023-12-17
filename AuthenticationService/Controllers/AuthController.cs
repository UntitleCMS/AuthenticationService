using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;

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
        var claims = new List<Claim>
    {
        // 'subject' claim which is required
        new(OpenIddictConstants.Claims.Subject, result.Principal.GetClaim("sub") ?? ""),
        new Claim(OpenIddictConstants.Claims.Name, result.Principal.GetClaim("name") ?? "").SetDestinations(OpenIddictConstants.Destinations.AccessToken),
        new("gh_token", result.Principal.GetClaim("token") ?? ""),
        new Claim("some claim", "some value").SetDestinations(OpenIddictConstants.Destinations.AccessToken)
    };

        var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Set requested scopes (this is not done automatically)
        claimsPrincipal.SetScopes(request.GetScopes());

        // Signing in with the OpenIddict authentiction scheme trigger OpenIddict to issue a code (which can be exchanged for an access token)
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

    }
}
