using AuthenticationService.Authentication.Profile;
using AuthenticationService.Authentication.Profile.Dto;
using AuthenticationService.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthenticationService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class ProfileController : ControllerBase
{
    private readonly ProfileService _ProfileService;
    private readonly UserManager<IdentityUser> _userManager;

    public ProfileController(
        ProfileService profileService,
        UserManager<IdentityUser> userManager)
    {
        _ProfileService = profileService;
        _userManager = userManager;
    }

    public string Username => User.Identity!.Name!;

    [HttpPost("avatar")]
    public async Task<IActionResult> AddAvatarAsync(
        [FromForm]AvatarRequestDto avatar)
    {
        Claim claim = new(
            Claims.Picture,
            avatar.URI!.ToString());

        var addClaimResault = await _ProfileService
            .AddOrUpdateClaim(Username, claim);

        return addClaimResault.Succeeded
            ? Ok()
            : BadRequest(addClaimResault.Errors);
    }


    [HttpPost("phone-number")]
    public async Task<IActionResult> AddPhoneNumber(
        [FromForm]PhoneNumberRequestDto phone)
    {
        Claim claim = new(
            Claims.PhoneNumber,
            phone.PhoneNumber);

        var addClaimResault = await _ProfileService
            .AddOrUpdateClaim(Username, claim);

        return addClaimResault.Succeeded
            ? Ok()
            : BadRequest();
    }


    [HttpGet("~/profiles")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProfile([FromQuery]IEnumerable<string> uid)
    {
        var a = uid.Select(i => i.ToGuid().ToString());
        return Ok(await _ProfileService.GetProfiles(a));
    }

    [HttpGet("~/allprofile")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProfile()
    {
        return Ok(_userManager.Users.Select(u=>new
        {
            id = u.Id,
            idb64 = (new Guid(u.Id)).ToBase64Url(),
            username = u.UserName
        }));
    }
}
