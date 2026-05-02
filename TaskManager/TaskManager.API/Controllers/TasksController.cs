using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Contracts;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Services;

namespace TaskManager.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;

    public TasksController(ITaskService tasks)
    {
        _tasks = tasks;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> List(CancellationToken cancellationToken)
    {
        var userId = GetUserIdOrThrow();
        var result = await _tasks.ListAsync(userId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{taskId:guid}")]
    public async Task<ActionResult<TaskDto>> Get(Guid taskId, CancellationToken cancellationToken)
    {
        var userId = GetUserIdOrThrow();
        var result = await _tasks.GetAsync(userId, taskId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserIdOrThrow();
        var created = await _tasks.CreateAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { taskId = created.Id }, created);
    }

    [HttpPut("{taskId:guid}")]
    public async Task<ActionResult<TaskDto>> Update(Guid taskId, [FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserIdOrThrow();
        var updated = await _tasks.UpdateAsync(userId, taskId, request, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> Delete(Guid taskId, CancellationToken cancellationToken)
    {
        var userId = GetUserIdOrThrow();
        await _tasks.DeleteAsync(userId, taskId, cancellationToken);
        return NoContent();
    }

    private Guid GetUserIdOrThrow()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var userId))
            throw new UnauthorizedAppException("Could not resolve the authenticated user from the token.");

        return userId;
    }
}
