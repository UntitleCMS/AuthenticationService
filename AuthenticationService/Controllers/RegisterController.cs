using AuthenticationService.Authentication.Register;
using AuthenticationService.Authentication.Register.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace AuthenticationService.Controllers;

//[Route("api/[controller]")]
[ApiController]
public class RegisterController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    private readonly RegisterService _registerService;

    public RegisterController
    (
        IOpenIddictApplicationManager applicationManager,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,

        RegisterService registerService)
    {
        _applicationManager = applicationManager;
        _userManager = userManager;
        _signInManager = signInManager;
        _registerService = registerService;
    }



    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromForm] RegisterRequestDto registerDTO)
    {
        var resault = await _registerService.NewUserAsync(registerDTO);

        if (resault.Succeeded)
            return Ok(registerDTO);

        return BadRequest(resault.Errors);
    }

}
