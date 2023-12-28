using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers;

[Route("api")]
[ApiController]
public class FastAuthEndpointController : ControllerBase
{
    public IActionResult Provider()
    {
        return Challenge(
            authenticationSchemes: "",
            properties: new AuthenticationProperties
            {
                RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                    Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
            });
    }
}
