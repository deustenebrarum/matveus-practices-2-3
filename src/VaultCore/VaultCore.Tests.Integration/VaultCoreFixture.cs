using Alba;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using VaultCore.Api.Services;
using Xunit;

namespace VaultCore.Tests.Integration;

public class VaultCoreFixture : IAsyncLifetime
{
    public IAlbaHost Host { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Host = await AlbaHost.For<Program>(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.ConfigureMarten(opts =>
                {
                    opts.Connection("Host=localhost;Port=5432;Database=vaultcore_test;Username=postgres;Password=postgres;");
                });
            });
        });

        using var scope = Host.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<SeedDataService>();
        await seeder.SeedAsync();
    }

    public async Task DisposeAsync()
    {
        await Host.DisposeAsync();
    }
}
