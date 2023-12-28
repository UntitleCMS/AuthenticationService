using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Entitis;

public class AppIdentityUser : IdentityUser
{
    public virtual string? OAuthType { get; set; }
    public virtual string? OAuthID { get; set; }
}
