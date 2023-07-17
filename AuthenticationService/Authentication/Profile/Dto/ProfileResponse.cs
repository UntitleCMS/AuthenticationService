namespace AuthenticationService.Authentication.Profile.Dto;

public class ProfileResponse
{
    public string ID { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Avatar => $"/avatar/{ID}";
}
