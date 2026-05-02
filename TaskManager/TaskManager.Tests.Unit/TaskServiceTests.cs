using Moq;
using TaskManager.Application.Abstractions;
using TaskManager.Application.Contracts;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Services;
using TaskManager.Domain;

namespace TaskManager.Tests.Unit;

public sealed class TaskServiceTests
{
    [Fact]
    public async Task ListAsync_MapsRepositoryItems_ToDtos()
    {
        var userId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var updatedAt = DateTime.UtcNow;
        var items = new List<TaskItem>
        {
            new(Guid.NewGuid(), userId, "A", "D", TaskItemStatus.InProgress, null, createdAt, updatedAt),
        };

        var tasks = new Mock<ITaskRepository>();
        tasks.Setup(x => x.ListByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var sut = new TaskService(tasks.Object);
        var result = await sut.ListAsync(userId, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("A", result[0].Title);
        Assert.Equal(TaskItemStatus.InProgress, result[0].Status);
    }

    [Fact]
    public async Task CreateAsync_WhenTitleEmpty_ThrowsAppValidationException()
    {
        var tasks = new Mock<ITaskRepository>();
        var sut = new TaskService(tasks.Object);

        await Assert.ThrowsAsync<AppValidationException>(() =>
            sut.CreateAsync(
                Guid.NewGuid(),
                new CreateTaskRequest("   ", null, TaskItemStatus.Todo, null),
                CancellationToken.None));
    }

    [Fact]
    public async Task GetAsync_WhenMissing_ThrowsNotFoundException()
    {
        var tasks = new Mock<ITaskRepository>();
        tasks.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        var sut = new TaskService(tasks.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            sut.GetAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task GetAsync_WhenFound_ReturnsDto()
    {
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var item = new TaskItem(
            taskId,
            userId,
            "Task",
            "Desc",
            TaskItemStatus.Todo,
            null,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow);

        var tasks = new Mock<ITaskRepository>();
        tasks.Setup(x => x.GetByIdAsync(taskId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var sut = new TaskService(tasks.Object);
        var result = await sut.GetAsync(userId, taskId, CancellationToken.None);

        Assert.Equal(taskId, result.Id);
        Assert.Equal("Task", result.Title);
        Assert.Equal(TaskItemStatus.Todo, result.Status);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ThrowsNotFoundException()
    {
        var tasks = new Mock<ITaskRepository>();
        tasks.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        var sut = new TaskService(tasks.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            sut.UpdateAsync(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new UpdateTaskRequest("x", null, TaskItemStatus.Todo, null),
                CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_WhenMissing_ThrowsNotFoundException()
    {
        var tasks = new Mock<ITaskRepository>();
        tasks.Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = new TaskService(tasks.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            sut.DeleteAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task DeleteAsync_WhenFound_Completes()
    {
        var tasks = new Mock<ITaskRepository>();
        tasks.Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new TaskService(tasks.Object);
        await sut.DeleteAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        tasks.Verify(
            x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_TrimsTitleAndDescription()
    {
        var userId = Guid.NewGuid();
        TaskItem? captured = null;
        var tasks = new Mock<ITaskRepository>();
        tasks.Setup(x => x.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Callback<TaskItem, CancellationToken>((t, _) => captured = t)
            .Returns(Task.CompletedTask);

        var sut = new TaskService(tasks.Object);
        var result = await sut.CreateAsync(
            userId,
            new CreateTaskRequest("  Title  ", "  Desc  ", TaskItemStatus.Todo, null),
            CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal("Title", captured!.Title);
        Assert.Equal("Desc", captured.Description);
        Assert.Equal(userId, captured.UserId);
        Assert.Equal("Title", result.Title);
    }

    [Fact]
    public async Task UpdateAsync_WhenValid_UpdatesValues()
    {
        var userId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var existing = new TaskItem(
            taskId,
            userId,
            "Old",
            "Old D",
            TaskItemStatus.Todo,
            null,
            DateTime.UtcNow.AddDays(-2),
            DateTime.UtcNow.AddDays(-2));

        TaskItem? updatedCaptured = null;
        var tasks = new Mock<ITaskRepository>();
        tasks.Setup(x => x.GetByIdAsync(taskId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        tasks.Setup(x => x.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Callback<TaskItem, CancellationToken>((t, _) => updatedCaptured = t)
            .Returns(Task.CompletedTask);

        var sut = new TaskService(tasks.Object);
        var dto = await sut.UpdateAsync(
            userId,
            taskId,
            new UpdateTaskRequest("  New  ", "  ND  ", TaskItemStatus.Done, null),
            CancellationToken.None);

        Assert.NotNull(updatedCaptured);
        Assert.Equal("New", updatedCaptured!.Title);
        Assert.Equal("ND", updatedCaptured.Description);
        Assert.Equal(TaskItemStatus.Done, updatedCaptured.Status);
        Assert.True(updatedCaptured.UpdatedAtUtc >= existing.UpdatedAtUtc);
        Assert.Equal(TaskItemStatus.Done, dto.Status);
    }
}
