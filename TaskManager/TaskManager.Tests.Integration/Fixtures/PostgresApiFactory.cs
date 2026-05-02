using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace TaskManager.Tests.Integration.Fixtures;

public sealed class PostgresApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public PostgresApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Default"] = _connectionString,
                    ["Jwt:Issuer"] = "TaskManager",
                    ["Jwt:Audience"] = "TaskManagerClients",
                    ["Jwt:Key"] = "local-dev-super-secret-key-min-32-chars-long!",
                    ["Jwt:ExpirationMinutes"] = "120",
                }!);
        });
    }
}
