using AuthenticationService.Authentication.Register.Model;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Authentication.Register;

public class RegisterService
{
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterService(
        UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> NewUserAsync(RegisterRequestDto data)
    {
        IdentityUser user = new()
        {
            UserName = data.Username,
            Email = data.Email,
            PhoneNumber = data.PhoneNumber
        };

        var createUserResalt = await _userManager.CreateAsync(user, data.Password);

        return createUserResalt;

    }

}
