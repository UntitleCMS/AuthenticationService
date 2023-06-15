using AuthenticationService.Datas;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;


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
            // Enable endpoint.
            options.SetTokenEndpointUris("connect/token");
            options.SetUserinfoEndpointUris("connect/userinfo");

            // Enable flow.
            options.AllowPasswordFlow();
            options.AllowRefreshTokenFlow();

            // Allow no client_id request
            options.AcceptAnonymousClients();

            // Register the signing and encryption credentials.
            options
               .AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate()
               .DisableAccessTokenEncryption();


            // Using reference tokens means the actual access and refresh tokens
            // are stored in the database and different tokens, referencing the actual
            // tokens (in the db), are used in request headers. The actual tokens are not
            // made public.
            //options.UseReferenceAccessTokens();
            //options.UseReferenceRefreshTokens();

            options
                .RegisterScopes
                (
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.Roles,
                    OpenIddictConstants.Scopes.Phone
                );

            // Set the lifetime of your tokens
            //options.SetAccessTokenLifetime(TimeSpan.FromMinutes(5));
            options.SetAccessTokenLifetime(TimeSpan.FromDays(1));
            options.SetRefreshTokenLifetime(TimeSpan.FromDays(2));

            //options.SetAccessTokenLifetime(TimeSpan.FromSeconds(30));
            //options.SetRefreshTokenLifetime(TimeSpan.FromMinutes(30));

            //options.SetIssuer("https://172.28.64.1:4434");

            // Register the ASP.NET Core host and configure the ASP.NET Core options.
            options.UseAspNetCore()
                   .EnableTokenEndpointPassthrough();
        })

        // Register the OpenIddict validation components.
        .AddValidation(options =>
        {
            // Import the configuration from the local OpenIddict server instance.
            options.UseLocalServer();

            // Register the ASP.NET Core host.
            options.UseAspNetCore();
        })

        // github
        .AddClient(options =>
        {
            options
                .AllowAuthorizationCodeFlow();
            options
                .AddDevelopmentEncryptionCertificate()
                .AddDevelopmentSigningCertificate();
            options
                .UseAspNetCore()
                .EnableRedirectionEndpointPassthrough();
            options.UseWebProviders()
                .UseGitHub(options =>
                {
                    options
                        .SetClientId("b4b68f83d48d756f33e0")
                        .SetClientSecret("8c584df75e3f9ce3a554641946cc2faed50b34a8")
                        .SetRedirectUri("callback/login/github");
                });
        });

        // Register the worker responsible of seeding the database with the sample clients.
        // Note: in a real world application, this step should be part of a setup script.
        //services.AddHostedService<Worker>(); 
    }
}
