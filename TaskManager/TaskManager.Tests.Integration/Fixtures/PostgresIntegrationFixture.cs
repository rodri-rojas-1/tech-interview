using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace TaskManager.Tests.Integration.Fixtures;

/// <summary>
/// Provides a <see cref="WebApplicationFactory{TEntryPoint}"/> against PostgreSQL.
/// Resolution order:
/// 1) <c>TaskManager.API/appsettings.Testing.json</c> connection string
/// 2) <c>TASKMANAGER_INTEGRATION_CONNECTION_STRING</c> environment variable
/// 3) Docker Testcontainers PostgreSQL
/// If none is available,
/// <see cref="Factory"/> is null and <see cref="SkipReason"/> explains why (use with SkippableFact).
/// </summary>
public sealed class PostgresIntegrationFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public WebApplicationFactory<Program>? Factory { get; private set; }

    public string? SkipReason { get; private set; }

    public async Task InitializeAsync()
    {
        var fileCs = ReadTestingConfigConnectionString();
        if (!string.IsNullOrWhiteSpace(fileCs))
        {
            Factory = new PostgresApiFactory(fileCs.Trim());
            return;
        }

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
                "Provide a testing connection in TaskManager.API/appsettings.Testing.json, " +
                "or set environment variable TASKMANAGER_INTEGRATION_CONNECTION_STRING, " +
                "or start Docker Desktop. " +
                "Connection string example: Host=localhost;Port=5432;Database=taskmanager_test;Username=postgres;Password=...";
            Factory = null;
        }
    }

    private static string? ReadTestingConfigConnectionString()
    {
        var apiConfig = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "TaskManager.API", "appsettings.Testing.json"));

        if (!File.Exists(apiConfig))
            return null;

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(apiConfig, optional: false, reloadOnChange: false)
            .Build();

        return configuration.GetConnectionString("Default");
    }

    public async Task DisposeAsync()
    {
        if (Factory is not null)
            await Factory.DisposeAsync();
        if (_container is not null)
            await _container.DisposeAsync();
    }
}
