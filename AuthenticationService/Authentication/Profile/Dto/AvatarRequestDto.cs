using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Authentication.Profile.Dto;

public class AvatarRequestDto
{
    [Required]
    public Uri? URI { get; set; } 
}
