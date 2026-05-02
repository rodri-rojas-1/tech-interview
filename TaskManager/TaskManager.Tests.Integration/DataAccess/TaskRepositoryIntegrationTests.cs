using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Abstractions;
using TaskManager.Domain;
using TaskManager.Tests.Integration.Fixtures;
using Xunit;

namespace TaskManager.Tests.Integration.DataAccess;

[Collection("postgres")]
public sealed class TaskRepositoryIntegrationTests
{
    private readonly PostgresIntegrationFixture _fixture;

    public TaskRepositoryIntegrationTests(PostgresIntegrationFixture fixture)
    {
        _fixture = fixture;
    }

    [SkippableFact]
    public async Task Add_List_Get_Update_Delete_roundtrip()
    {
        Skip.If(_fixture.Factory is null, _fixture.SkipReason);
        using var scope = _fixture.Factory!.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var tasks = scope.ServiceProvider.GetRequiredService<ITaskRepository>();

        var user = new User(
            Guid.NewGuid(),
            "task-repo@local",
            "hash",
            DateTime.UtcNow);
        await users.AddAsync(user, CancellationToken.None);

        var now = DateTime.UtcNow;
        var task = new TaskItem(
            Guid.NewGuid(),
            user.Id,
            "Integration title",
            "Desc",
            TaskItemStatus.Todo,
            null,
            now,
            now);

        await tasks.AddAsync(task, CancellationToken.None);

        var list = await tasks.ListByUserAsync(user.Id, CancellationToken.None);
        Assert.Single(list);
        Assert.Equal("Integration title", list[0].Title);

        var loaded = await tasks.GetByIdAsync(task.Id, user.Id, CancellationToken.None);
        Assert.NotNull(loaded);

        var updated = loaded! with
        {
            Title = "Updated",
            Status = TaskItemStatus.InProgress,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        await tasks.UpdateAsync(updated, CancellationToken.None);

        var afterUpdate = await tasks.GetByIdAsync(task.Id, user.Id, CancellationToken.None);
        Assert.Equal("Updated", afterUpdate!.Title);
        Assert.Equal(TaskItemStatus.InProgress, afterUpdate.Status);

        var deleted = await tasks.DeleteAsync(task.Id, user.Id, CancellationToken.None);
        Assert.True(deleted);

        var gone = await tasks.GetByIdAsync(task.Id, user.Id, CancellationToken.None);
        Assert.Null(gone);
    }
}
