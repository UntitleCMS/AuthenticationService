using AuthenticationService.Authentication.Profile.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthenticationService.Authentication.Profile;

public class ProfileService
{ 
    private readonly UserManager<IdentityUser> _userManager;

    public ProfileService(
        UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public Task<IEnumerable<ProfileResponse>> GetProfiles(IEnumerable<string> ids)
    {
        var a = _userManager.Users
            .Where(u => ids.Contains(u.Id))
            .AsNoTracking()
            .Select(u => new ProfileResponse()
            {
                ID = u.Id,
                Username = u.UserName!,
                Avatar = $"/avatar/{u.Id}"
            }) ;
        return Task.FromResult(a.AsEnumerable());
    }

    public async Task<IdentityResult> AddOrUpdateClaim( string username, Claim claim)
    {
        var user = await _userManager.FindByNameAsync(username)
                ?? throw new Exception("User Not Found!"); 
        return await AddOrUpdateClaim(user, claim);
    }
    public async Task<IdentityResult> AddOrUpdateClaim( IdentityUser user, Claim claim)
    {
        var c = (await _userManager.GetClaimsAsync(user))
            .FirstOrDefault(c=>c.Type == claim.Type);

        var addClaimResault = c is null
            ? await _userManager.AddClaimAsync(user, claim)
            : await _userManager.ReplaceClaimAsync(user, c, claim);

        return addClaimResault;
    }
}
