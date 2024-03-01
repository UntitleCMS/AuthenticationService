using AuthenticationService.Datas;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthenticationService
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("console") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "console",
                    //ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
                    DisplayName = "My client application",
                    RedirectUris = {
                        new Uri("https://p.villsource.tk"),
                        new Uri("http://local.villsource.tk"),
                        new Uri("http://local.villsource.tk:4200"),
                        new Uri("https://local.villsource.tk"),
                        new Uri("http://localhost:4200"),
                        new Uri("http://localhost"),
                        new Uri("https://localhost"),
                        new Uri("https://blog.villsource.net"),
                        new Uri("http://blog.villsource.net")
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Authorization,

                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.GrantTypes.AuthorizationCode,

                        Permissions.GrantTypes.Password,
                        Permissions.GrantTypes.RefreshToken,

                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,

                        Permissions.ResponseTypes.Code
                    }
                });
            }
            if (await manager.FindByClientIdAsync("postman") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "postman",
                    //ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
                    DisplayName = "My client application",
                    RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Authorization,

                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.GrantTypes.AuthorizationCode,

                        Permissions.GrantTypes.Password,
                        Permissions.GrantTypes.RefreshToken,

                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles,

                        Permissions.ResponseTypes.Code
                    }
                });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
