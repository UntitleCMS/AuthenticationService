using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthenticationService.Authentication;

public static class ScopeConstants
{
    public static IEnumerable<string> SupportedScope => new[]
        {
            Scopes.OpenId,
            Scopes.Email,
            Scopes.Phone,
            Scopes.Profile,
            Scopes.Roles,
            Scopes.OfflineAccess
        };
}
