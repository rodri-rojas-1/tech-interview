using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Abstractions;
using TaskManager.Domain;
using TaskManager.Tests.Integration.Fixtures;
using Xunit;

namespace TaskManager.Tests.Integration.DataAccess;

[Collection("postgres")]
public sealed class UserRepositoryIntegrationTests
{
    private readonly PostgresIntegrationFixture _fixture;

    public UserRepositoryIntegrationTests(PostgresIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    [SkippableFact]
    public async Task Add_and_GetByEmail_roundtrip()
    {
        Skip.If(_fixture.Factory is null, _fixture.SkipReason);
        using var scope = _fixture.Factory!.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var user = new User(
            Guid.NewGuid(),
            "repo-test@local",
            "hash",
            DateTime.UtcNow);

        await users.AddAsync(user, CancellationToken.None);

        var loaded = await users.GetByEmailAsync("repo-test@local", CancellationToken.None);

        Assert.NotNull(loaded);
        Assert.Equal(user.Id, loaded.Id);
        Assert.Equal("repo-test@local", loaded.Email);
    }
}
