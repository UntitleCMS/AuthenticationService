using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Claims;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationService.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthorizationController(IOpenIddictApplicationManager applicationManager, UserManager<IdentityUser> userManager)
        {
            _applicationManager = applicationManager;
            _userManager = userManager;
        }

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request
                = HttpContext.GetOpenIddictServerRequest()
                ?? throw new InvalidOperationException("OpenIddictServerRequest is null");


            //if (!request.IsClientCredentialsGrantType())
            //{
            //    throw new NotImplementedException("The specified grant is not implemented.");
            //}

            // Create a new ClaimsIdentity containing the claims that
            // will be used to create an id_token, a token or a code.
            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);


            if (request.IsClientCredentialsGrantType())
            {
                // Note: the client credentials are automatically validated by OpenIddict:
                // if client_id or client_secret are invalid, this action won't be invoked.
                var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                    throw new InvalidOperationException("The application cannot be found.");

                // Use the client_id as the subject identifier.
                identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application));
                identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));
            }


            // Use the client_id as the subject identifier.
            else if (request.IsPasswordGrantType())
            {
                var usr = await _userManager.FindByNameAsync(request.Username!); 
                if (usr == null || usr.PasswordHash != request.Password)
                    return BadRequest("Login fail.");

                identity.SetClaim(Claims.Subject, usr.Id);
                identity.SetClaim(Claims.Name, usr.UserName);
            }

            identity.SetDestinations(static claim => claim.Type switch
            {
                // Allow the "name" claim to be stored in both the access and identity tokens
                // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                Claims.Name when claim.Subject.HasScope(Scopes.Profile)
                    => new[] { Destinations.AccessToken, Destinations.IdentityToken },

                // Otherwise, only store the claim in the access tokens.
                _ => new[] { Destinations.AccessToken }
            });

            return SignIn
            (
                new ClaimsPrincipal(identity),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
            );
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDTO registerDTO)
        {
            var usr = await _userManager.FindByNameAsync(registerDTO.Username);
            if (usr != null) return BadRequest($"username '{registerDTO.Username}' is already existed.");

            IdentityUser user = new()
            { 
                UserName = registerDTO.Username,
                PasswordHash = registerDTO.Password
            };
            await _userManager.CreateAsync(user);

            return Ok(registerDTO);
        }
    }

    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
