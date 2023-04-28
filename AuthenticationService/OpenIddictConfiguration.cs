using AuthenticationService.Datas;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace AuthenticationService;

public static class OpenIddictConfiguration
{
    public static void AddMyOpendIddictConfiguration(this IServiceCollection services)
    {
        // OpenIdDict
        services.AddOpenIddict()

        // Register the OpenIddict core components.
        .AddCore(options =>
        {
            // Configure OpenIddict to use the Entity Framework Core stores and models.
            // Note: call ReplaceDefaultEntities() to replace the default entities.
            options.UseEntityFrameworkCore()
                   .UseDbContext<TmpDataContext>();
        })

        // Register the OpenIddict server components.
        .AddServer(options =>
        {
            // Enable the token endpoint.
            options.SetTokenEndpointUris("connect/token");

            // Enable the client credentials flow.
            options.AllowClientCredentialsFlow();

            // Register the signing and encryption credentials.
            options.AddDevelopmentEncryptionCertificate()
                   .AddDevelopmentSigningCertificate()
                   .DisableAccessTokenEncryption();

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
        services.AddHostedService<Worker>(); 
    }
}
