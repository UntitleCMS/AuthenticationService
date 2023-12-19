using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Entitis;

public enum OAuthType
{
    Github, Facebook, Google
}

public class AppIdentityUser : IdentityUser
{
    public virtual OAuthType? OAuthType { get; set; }
    public virtual string? OAuthID { get; set; } = string.Empty;
}
