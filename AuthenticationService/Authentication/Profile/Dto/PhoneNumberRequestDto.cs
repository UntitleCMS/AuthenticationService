using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Authentication.Profile.Dto;

public class PhoneNumberRequestDto
{
    [Required, Phone]
    public String PhoneNumber { get; set; } = string.Empty;
}
