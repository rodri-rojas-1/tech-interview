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
}
