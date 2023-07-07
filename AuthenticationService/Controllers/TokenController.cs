using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Claims;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using AuthenticationService.Authentication.Token;

namespace AuthenticationService.Controllers;

public class TokenController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    private readonly TokenService _tokenService;

    public TokenController
    (
        IOpenIddictApplicationManager applicationManager,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager
,
        TokenService tokenService)
    {
        _applicationManager = applicationManager;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("token"), Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request
            = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("OpenIddictServerRequest is null");


        ClaimsPrincipal principal;

        switch (request.GrantType)
        {
            case GrantTypes.RefreshToken:
                principal = await _tokenService.Refresh();
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            case GrantTypes.Password:
                principal = await _tokenService.Password(); 
                return SignIn( principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

       throw new NotImplementedException("The specified grant type is not implemented.");
    }

}
