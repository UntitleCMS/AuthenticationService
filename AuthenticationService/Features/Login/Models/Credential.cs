namespace AuthenticationService.Features.Login.Models;

public class Credential
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Provider { get; set; }
}
