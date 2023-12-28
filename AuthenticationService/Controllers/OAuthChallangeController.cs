using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers;

[Route("oauth")]
[ApiController]
public class OAuthChallangeController : ControllerBase
{
    [HttpGet("github")]
    public IActionResult GithubChallange()
    {
        return Challenge(
            new AuthenticationProperties()
            {
                RedirectUri = "/"
            },
            "github"
        );
    }

    [HttpGet("facebook")]
    public IActionResult FacebookChallange()
    {
        return Challenge(
            new AuthenticationProperties()
            {
                RedirectUri = "/"
            },
            "facebook"
        );
    }
    [HttpGet("google")]
    public IActionResult GoogleChallange()
    {
        return Challenge(
            new AuthenticationProperties()
            {
                RedirectUri = "/"
            },
            "google"
        );
    }

}
