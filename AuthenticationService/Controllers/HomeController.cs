using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace AuthenticationService.Controllers;

[ApiController]
public class HomeController : ControllerBase
{
    [Route("/"), HttpGet, HttpOptions]
    [Authorize( AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    //[Authorize( AuthenticationSchemes = "cookie")]
    public async Task<IActionResult> Home()
    {
        var c = User.Claims.Select(i=>(i.Type, i.Value)).ToList();
        return Ok(c);
    }
}
