using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace TaskManager.Tests.Integration.Fixtures;

/// <summary>
/// Provides a <see cref="WebApplicationFactory{TEntryPoint}"/> against PostgreSQL.
/// If neither Docker nor <c>TASKMANAGER_INTEGRATION_CONNECTION_STRING</c> is available,
/// <see cref="Factory"/> is null and <see cref="SkipReason"/> explains why (use with SkippableFact).
/// </summary>
public sealed class PostgresIntegrationFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public WebApplicationFactory<Program>? Factory { get; private set; }

    public string? SkipReason { get; private set; }

    public async Task InitializeAsync()
    {
        var envCs = Environment.GetEnvironmentVariable("TASKMANAGER_INTEGRATION_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(envCs))
        {
            Factory = new PostgresApiFactory(envCs.Trim());
            return;
        }

        try
        {
            _container = new PostgreSqlBuilder("postgres:16-alpine").Build();
            await _container.StartAsync();
            Factory = new PostgresApiFactory(_container.GetConnectionString());
        }
        catch (DockerUnavailableException)
        {
            SkipReason =
                "PostgreSQL integration tests skipped: Docker is not running. " +
                "Start Docker Desktop, or set environment variable TASKMANAGER_INTEGRATION_CONNECTION_STRING " +
                "to a PostgreSQL connection string (e.g. Host=localhost;Port=5432;Database=taskmanager_test;Username=postgres;Password=...).";
            Factory = null;
        }
    }

    public async Task DisposeAsync()
    {
        if (Factory is not null)
            await Factory.DisposeAsync();
        if (_container is not null)
            await _container.DisposeAsync();
    }
}
