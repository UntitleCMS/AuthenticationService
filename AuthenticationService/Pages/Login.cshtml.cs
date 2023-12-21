using AuthenticationService.Features.Login;
using AuthenticationService.Features.Login.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AuthenticationService.Pages;

public class LoginModel : PageModel
{
    private UsernameLoginService _usernameLogin;

    public LoginModel(UsernameLoginService usernameLoginService)
    {
        _usernameLogin = usernameLoginService;
    }

    [BindProperty]
    public Credential? C { get; set; } = new();
    public string Message { get; set; } = string.Empty;

    public async Task<IActionResult> OnGet()
    {
        var res = await HttpContext.AuthenticateAsync("cookie");

        // Render Login Page if Not login
        if (!res.Succeeded)
            return Page();

        // Re-signin to complete Challenge if loged in
        var claims = new Claim[] { new("dummy", "dummy") };
        var identity = new ClaimsIdentity(claims, "dummy");
        var property = new AuthenticationProperties();

        if (!Request.Query.Any(i => i.Key == "ReturnUrl"))
            property.RedirectUri = HttpContext.Request.PathBase;

        return SignIn(new(identity), property, "dummy");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var property = new AuthenticationProperties();

        if (!Request.Query.Any(i => i.Key == "ReturnUrl"))
            property.RedirectUri = HttpContext.Request.PathBase;

        if (C!.Provider?.CompareTo("login") == 0)
        {
            if (C.Username is null || C.Password is null)
                return IncorrectPasswordOrUsername();

            var claims = await _usernameLogin.GetClaimsAsync(C.Username, C.Password);

            if (!claims.IsSuccess)
                return IncorrectPasswordOrUsername();

            var identity = new ClaimsIdentity(claims.Value, "cookie");
            return SignIn(new(identity), property, "cookie");
        }

        var provider = new string[] { "google", "facebook", "github" };
        if (!provider.Contains(C.Provider))
            return NotSupportLoginMehtod();

        return Challenge(property, C.Provider!);
    }

    private IActionResult IncorrectPasswordOrUsername()
    {
        Message = "Incorrect Username or Password";
        return Page();
    }

    private IActionResult NotSupportLoginMehtod()
    {
        Message = "Not Supported Login Method";
        return Page();
    }

}
