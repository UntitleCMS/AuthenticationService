using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.Security.Claims;

namespace AuthenticationService.Pages;

public class LoginModel : PageModel
{
    [BindProperty]
    public Credential? C { get; set; } = new();

    public async Task<IActionResult> OnGet()
    {
        var res = await HttpContext.AuthenticateAsync("cookie");

        Console.WriteLine(User.Identity?.IsAuthenticated);

        if (User.Identity is null || !User.Identity.IsAuthenticated)
            return Page();

        Console.WriteLine("Auth!!!!!!!!!!!!!!!!!!!!!!!!!");

        var i = new ClaimsIdentity(res.Principal!.Claims, "cookie");
        var p = new AuthenticationProperties();

        if (!Request.Query.Any(i => i.Key == "ReturnUrl"))
            p.RedirectUri = "/";

        return SignIn(new ClaimsPrincipal(i), p, "cookie");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Console.WriteLine(C.Provider);

        //var c = new List<Claim>()
        //{
        //    new("sub","tmp"),
        //    new("name","tmp"),
        //};

        //var i = new ClaimsIdentity(c, "cookie");

        //return SignIn(new ClaimsPrincipal(i), "cookie");

        var provider = new string[] { "google","facebook", "github" };
        if (!provider.Contains(C.Provider))
        {
            return BadRequest();
        }

        return Challenge(authenticationSchemes: C.Provider!, properties: new()
        {
            AllowRefresh = true,
        });
    }

}

public class Credential
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Provider { get; set; }
}
