using AuthenticationService.Authentication.Profile;
using AuthenticationService.Authentication.Profile.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthenticationService.Controllers
{
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
            [FromBody]AvatarRequestDto avatar)
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
            [FromBody]PhoneNumberRequestDto phone)
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

    }
}
