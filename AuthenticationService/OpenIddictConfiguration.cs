using AuthenticationService.Datas;
using AuthenticationService.Entitis;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;


namespace AuthenticationService;

// some code from https://nwb.one/blog/openid-connect-dotnet-5
public static class OpenIddictConfiguration
{
    public static void AddMyOpendIddictConfiguration(this IServiceCollection services)
    {
        services.AddOpenIddict()

        .AddCore(options =>
        {
            options.UseEntityFrameworkCore()
                   .UseDbContext<AppDbContext>();
        })

        .AddServer(options =>
        {
            options.AllowClientCredentialsFlow();
            options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

            options.SetTokenEndpointUris("token");
            options.SetAuthorizationEndpointUris("auth");
            options.SetUserinfoEndpointUris("me");

            options
               .AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate()
               .DisableAccessTokenEncryption();

            options.UseAspNetCore()
                   .EnableTokenEndpointPassthrough()
                   .EnableAuthorizationEndpointPassthrough()
                   .DisableTransportSecurityRequirement();
        })

        .AddValidation(options =>
        {
            options.UseLocalServer();
            options.UseAspNetCore();
        });

        services.AddHostedService<Worker>();

        // Add ASP.NET Identity
        services.AddIdentity<AppIdentityUser, AppIdentityRole>()
            .AddSignInManager()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddUserManager<UserManager<AppIdentityUser>>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
        });

        services
            //.AddAuthentication("cookie")
            .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "cookie";
            options.DefaultScheme = OpenIddictConstants.Schemes.Bearer;
            //options.DefaultChallengeScheme = OpenIddictConstants.Schemes.Bearer;
            options.DefaultChallengeScheme = "github";
        })
            .AddCookie("cookie", o =>
            {
                o.ExpireTimeSpan = TimeSpan.FromSeconds(5);
                o.SlidingExpiration = false;
                o.LoginPath = "/login";
            })
            .AddCookie("dummy",o=>o.LoginPath = "/login")
            .AddOAuth("github", o =>
            {
                o.SignInScheme = "cookie";
                o.SaveTokens = true;

                o.ClientId = "b4b68f83d48d756f33e0";
                o.ClientSecret = "8c584df75e3f9ce3a554641946cc2faed50b34a8";

                o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                o.TokenEndpoint = "https://github.com/login/oauth/access_token";
                o.CallbackPath = "/oauth/cb/github";
                o.UserInformationEndpoint = "https://api.github.com/user";

                o.ClaimActions.MapJsonKey("sub", "id");
                o.ClaimActions.MapJsonKey("name", "login");

                o.Events.OnCreatingTicket = async ctx =>
                {
                    using var req = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
                    req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ctx.AccessToken);
                    using var res = await ctx.Backchannel.SendAsync(req);
                    var user = await res.Content.ReadFromJsonAsync<JsonElement>();
                    ctx.RunClaimActions(user);
                };
            })
            .AddOAuth("facebook", o =>
            {
                o.SignInScheme = "cookie";
                o.SaveTokens = true;

                o.ClientId = "344722471526711";
                o.ClientSecret = "f430ac494e6b3d9d809404d77b013047";


                o.CallbackPath = "/oauth/cb/facebook";
                o.AuthorizationEndpoint = "https://www.facebook.com/v18.0/dialog/oauth";
                o.TokenEndpoint = "https://graph.facebook.com/v18.0/oauth/access_token";
                o.UserInformationEndpoint = "https://graph.facebook.com/v16.0/me";

                o.ClaimActions.MapJsonKey("sub", "id");
                o.ClaimActions.MapJsonKey("name", "name");

                o.Events.OnCreatingTicket = async ctx =>
                {
                    using var req = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
                    req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ctx.AccessToken);
                    using var res = await ctx.Backchannel.SendAsync(req);
                    var user = await res.Content.ReadFromJsonAsync<JsonElement>();
                    ctx.RunClaimActions(user);
                };

            })
            .AddOAuth("google", o =>
            {
                o.SignInScheme = "cookie";
                o.SaveTokens = true;

                o.ClientId = "242349616268-1nl5f43uqea77qcpcts5aa0bbflks331.apps.googleusercontent.com";
                o.ClientSecret = "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";

                o.CallbackPath = "/oauth/cb/google";
                o.AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
                o.TokenEndpoint = "https://oauth2.googleapis.com/token";
                o.UserInformationEndpoint = "https://openidconnect.googleapis.com/v1/userinfo";
                o.Scope.Add("openid");
                o.Scope.Add("email");
                o.Scope.Add("profile");

                o.ClaimActions.MapJsonKey("sub", "sub");
                o.ClaimActions.MapJsonKey("name", "name");
                o.ClaimActions.MapJsonKey("email", "email");

                o.Events.OnCreatingTicket = async ctx =>
                {
                    using var req = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
                    req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ctx.AccessToken);
                    using var res = await ctx.Backchannel.SendAsync(req);
                    var user = await res.Content.ReadFromJsonAsync<JsonElement>();
                    ctx.RunClaimActions(user);
                };

            });


    }
}
