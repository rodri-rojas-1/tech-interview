using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Controllers;
using TaskManager.Application.Contracts;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Services;
using TaskManager.Domain;

namespace TaskManager.Tests.Unit;

public sealed class TasksControllerTests
{
    private static readonly Guid UserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task List_returns_ok_with_tasks()
    {
        var tasks = new Mock<ITaskService>();
        var dtos = new List<TaskDto>
        {
            new(
                Guid.NewGuid(),
                "T",
                null,
                TaskItemStatus.Todo,
                null,
                DateTime.UtcNow,
                DateTime.UtcNow),
        };
        tasks.Setup(x => x.ListAsync(UserId, It.IsAny<CancellationToken>())).ReturnsAsync(dtos);

        var sut = CreateController(tasks.Object);
        var result = await sut.List(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(dtos, ok.Value);
    }

    [Fact]
    public async Task Get_returns_ok()
    {
        var taskId = Guid.NewGuid();
        var tasks = new Mock<ITaskService>();
        var dto = new TaskDto(
            taskId,
            "T",
            null,
            TaskItemStatus.Done,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);
        tasks.Setup(x => x.GetAsync(UserId, taskId, It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        var sut = CreateController(tasks.Object);
        var result = await sut.Get(taskId, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(dto, ok.Value);
    }

    [Fact]
    public async Task Create_returns_created()
    {
        var tasks = new Mock<ITaskService>();
        var created = new TaskDto(
            Guid.NewGuid(),
            "New",
            null,
            TaskItemStatus.Todo,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);
        var req = new CreateTaskRequest("New", null, TaskItemStatus.Todo, null);
        tasks.Setup(x => x.CreateAsync(UserId, req, It.IsAny<CancellationToken>())).ReturnsAsync(created);

        var sut = CreateController(tasks.Object);
        var result = await sut.Create(req, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Same(created, createdResult.Value);
    }

    [Fact]
    public async Task Delete_returns_no_content()
    {
        var taskId = Guid.NewGuid();
        var tasks = new Mock<ITaskService>();
        tasks.Setup(x => x.DeleteAsync(UserId, taskId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sut = CreateController(tasks.Object);
        var result = await sut.Delete(taskId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task List_without_user_claim_throws_unauthorized_app_exception()
    {
        var tasks = new Mock<ITaskService>();
        var sut = new TasksController(tasks.Object);
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() },
        };

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            sut.List(CancellationToken.None));
    }

    private static TasksController CreateController(ITaskService svc)
    {
        var c = new TasksController(svc);
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, UserId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        c.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) },
        };
        return c;
    }
}
