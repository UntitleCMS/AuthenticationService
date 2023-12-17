using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Authorize( AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public class PrivateController : ControllerBase
    {
        [HttpGet("private")]
        public IActionResult Get()
        {
            return Ok("Bravo");
        }
    }
}
