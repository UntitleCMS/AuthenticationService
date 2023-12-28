using AuthenticationService.Entitis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Core.Extensions;

public static class OAuthUserManagerExtension
{
    public static AppIdentityUser? FindByOAuth(this UserManager<AppIdentityUser> um, string oAuthID, string? oAuthType = default)
    {
        var u = um.Users
            .Where(u => u.OAuthID == oAuthID)
            .ToList();

        if (u.IsNullOrEmpty())
            return null;

        if (u.Count == 1)
            return u.First();

        return u.Where(i=>i.OAuthType==oAuthType)
            .FirstOrDefault();
    }
}
