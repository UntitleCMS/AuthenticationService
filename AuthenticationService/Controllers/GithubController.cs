using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using OpenIddict.Client.AspNetCore;
using Polly;

namespace AuthenticationService.Controllers
{
    [ApiController]
    public class GithubController : ControllerBase
    {
        [HttpGet("challenge")]
        public IActionResult ChallengeGithub()
        {
            return Challenge(null, new[]
            {
                OpenIddictClientAspNetCoreDefaults.AuthenticationScheme
            });
        }

        [HttpPost, HttpGet, Route("callback/login/github")]
        public async Task<IActionResult> Github()
        {
            var result = await HttpContext.AuthenticateAsync
                (OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

            return Ok(string.Format("{0} has {1} public repositories.",
                result.Principal!.FindFirst("name")!.Value,
                result.Principal!.FindFirst("public_repos")!.Value));
        }
    }
}
