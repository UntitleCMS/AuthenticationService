using AuthenticationService.Extentions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthenticationService.Authentication.Token;

public class TokenService
{ 
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public TokenService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext HttpContext =>
        _httpContextAccessor.HttpContext
        ?? throw new Exception("HttpContext Not Found!");
    private OpenIddictRequest Request =>
                HttpContext.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException("OpenIddictServerRequest is null");

    public async Task<ClaimsPrincipal> Password()
    {
        string username = Request.Username!;
        string password = Request.Password!;

        IdentityUser user
            = await _userManager.FindByNameAsync(username)
            ?? throw new Exception($"Username {username} not found."); 

        var isPassingPassword = await _userManager.CheckPasswordAsync(user, password); 

        if (!isPassingPassword)
            throw new Exception("Wrong Password!");

        ClaimsPrincipal principal = await _signInManager.CreateUserPrincipalAsync(user);

        principal.SetScopes( ScopeConstants
            .SupportedScope
            .Intersect(Request.GetScopes()));

        principal.AddClaim("sub-b64", new Guid(user.Id).ToBase64Url() );

        principal?.SetDestinations(static claim => claim.Type switch
        {
            Claims.Name or
            Claims.Email
            when claim.Subject!.HasScope(Scopes.Profile)
                => new[] { Destinations.AccessToken, Destinations.IdentityToken },

            Claims.Picture or 
            Claims.Birthdate or
            Claims.PhoneNumber
                => new[] { Destinations.IdentityToken }, 

            _ => new[] { Destinations.AccessToken }
        });

        return principal!;
    }

    public async Task<ClaimsPrincipal> Refresh()
    { 
        return (await HttpContext
            .AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))
            .Principal
            ?? throw new Exception("Can't refresh.");
    }
}
