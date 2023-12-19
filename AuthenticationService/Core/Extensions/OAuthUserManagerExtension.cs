using AuthenticationService.Entitis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Core.Extensions;

public static class OAuthUserManagerExtension
{
    public static Task<List<AppIdentityUser>> FindByOAuth(this UserManager<AppIdentityUser> um, string oAuthID, OAuthType? oAuthType = default)
    {
        var u = um.Users.Where(u => u.OAuthID == oAuthID);

        if (oAuthType is not null)
            u = u.Where(u => u.OAuthType == oAuthType);

        var res = u.ToListAsync();
        return res;
    }
}
