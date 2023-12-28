using AuthenticationService.Core.Exceptions;
using AuthenticationService.Core.Extensions;
using AuthenticationService.Entitis;
using AuthenticationService.Features.Login.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthenticationService.Features.Login;

public class UsernameLoginService
{
    private UserManager<AppIdentityUser> _userManager;

    public UsernameLoginService(UserManager<AppIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UsernameLoginResult> GetClaimsAsync(string username, string password)
    {
        var res = new UsernameLoginResult() { 
            Errors = new UsernamePasswordException("Wrong username or password")
        };

        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
            return res;

        var isVerified = await _userManager.CheckPasswordAsync(user, password);
        if (!isVerified)
            return res;

        var c = new List<Claim>()
            {
                new("name", user.UserName!),
                new("sub",user.Id),
            };

        res.Value = c;
        res.Errors = default;

        return res;
    }
}
