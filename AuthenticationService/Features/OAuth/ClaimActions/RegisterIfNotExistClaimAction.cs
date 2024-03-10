using AuthenticationService.Core.Extensions;
using AuthenticationService.Entitis;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthenticationService.Features.OAuth.ClaimActions;

public class RegisterIfNotExistClaimAction : ClaimAction
{
    private readonly UserManager<AppIdentityUser> _userManager;

    public RegisterIfNotExistClaimAction(UserManager<AppIdentityUser> userManager)
        : base("", "")
    {
        _userManager = userManager;
    }

    public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
    {
        Console.WriteLine("############# REGISTER IF NOT EXIST CLAIM ACTION ##################");
        var id = getID(issuer, userData);
        var user = GetAccount(id);

        identity.AddClaim(new(Claims.Subject, user.Result.Id));
        identity.AddClaim(new(Claims.Name, user.Result.UserName));
    }

    private async Task<AppIdentityUser> GetAccount(OAuthID id)
    {
        var user = _userManager.FindByOAuth(id.Sub, id.Issuer);

        if (user is null)
        {
            user = new()
            {
                OAuthID = id.Sub,
                OAuthType = id.Issuer,
                UserName = Regex.Replace(id.Name!.Trim(), @"[^a-zA-Z\d]", "_"),
            };
            var res = await _userManager.CreateAsync(user);
        }

        return user;
    }

    private OAuthID getID(string issuer, JsonElement userData)
    {
        var a = new OAuthID();
        a.Issuer = issuer;

        if (issuer == "github")
        {
            a.Sub = userData.GetProperty("id").GetDecimal().ToString();
            a.Name = userData.GetProperty("login").GetString();
        }
        else if (issuer == "facebook")
        {
            a.Sub = userData.GetProperty("id").ToString();
            a.Name = userData.GetProperty("name").GetString();
        }
        else if (issuer == "google")
        {
            a.Sub = userData.GetProperty("sub").GetString();
            a.Name = userData.GetProperty("name").GetString();
        }
        return a;
    }
}

public class OAuthID
{
    public string? Issuer { get; set; } = string.Empty;
    public string? Sub { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
}
