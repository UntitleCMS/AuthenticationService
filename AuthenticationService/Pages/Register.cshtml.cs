using AuthenticationService.Entitis;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<AppIdentityUser> _userManager;


        public RegisterModel(UserManager<AppIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public UserInfomation UserInfo { get; set; }
        public string Message { get; set; } = string.Empty;
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var u = await _userManager.CreateAsync(new()
            {
                UserName = UserInfo.UserName,
            }, UserInfo.Password);

            if (u.Succeeded)
                return Redirect(Request.PathBase + "/Login" + QueryString.Create(Request.Query.ToList()));

            else
            {
                var msg = u.Errors.Select(e => e.Description);
                Message = string.Join(",", msg);
                return Page();
            }
        }
    }

    public class UserInfomation
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
