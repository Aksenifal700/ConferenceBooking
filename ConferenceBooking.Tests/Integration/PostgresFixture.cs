using ConferenceBooking.DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Testcontainers.PostgreSql;

namespace ConferenceBooking.Tests.Integration;

public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;

    public PostgresFixture()
    {
        _container = new PostgreSqlBuilder("postgres:17").Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public AppDbContext CreateContext(params IInterceptor[] interceptors)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .AddInterceptors(interceptors)
            .Options;

        return new AppDbContext(options);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}