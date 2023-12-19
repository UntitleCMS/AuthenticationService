using AuthenticationService.Entitis;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.Json;

namespace AuthenticationService.Features.OAuth.ClaimActions;

public class RegisterIfNotExistClaimAction : ClaimAction
{
    private UserManager<AppIdentityUser> _userManager;

    public RegisterIfNotExistClaimAction(UserManager<AppIdentityUser> userManager) : base("", "")
    {
        _userManager = userManager;
    }

    public RegisterIfNotExistClaimAction(string claimType, string valueType) : base(claimType, valueType)
    {
    }

    public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
    {
        Console.WriteLine("############# REGISTER IF NOT EXIST CLAIM ACTION ##################");
        var x = getID(issuer, userData);
    }

    private void TryGetAccount(OAuthID id)
    {

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

        return a;
    }
}

public class OAuthID
{
    public string? Issuer { get; set; } = string.Empty;
    public string? Sub { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
}
