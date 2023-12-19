using AuthenticationService.Core.Models;
using System.Security.Claims;

namespace AuthenticationService.Features.Login.Models;

public class UsernameLoginResult : Result<List<Claim>>
{
}
