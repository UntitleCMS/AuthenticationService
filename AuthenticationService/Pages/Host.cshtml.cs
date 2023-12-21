using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthenticationService.Pages;

public class HostModel : PageModel
{
    public string Host { get; set; } = string.Empty;
    public void OnGet()
    {
        Host = HttpContext.Request.Host.Value;
    }
}
