using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Authentication.Register.Model;

public class RegisterRequestDto
{
    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(20)]
    public string Email { get; set; } = string.Empty;
}
