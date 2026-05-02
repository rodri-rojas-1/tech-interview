using TaskManager.Application.Abstractions;
using TaskManager.Application.Contracts;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Validation;
using TaskManager.Domain;

namespace TaskManager.Application.Services;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _tasks;

    public TaskService(ITaskRepository tasks)
    {
        _tasks = tasks;
    }

    public async Task<IReadOnlyList<TaskDto>> ListAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _tasks.ListByUserAsync(userId, cancellationToken);
        return items.Select(ToDto).ToList();
    }

    public async Task<TaskDto> GetAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await _tasks.GetByIdAsync(taskId, userId, cancellationToken);
        if (task is null)
            throw new NotFoundException("Task not found.");

        return ToDto(task);
    }

    public async Task<TaskDto> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        InputValidators.EnsureTaskTitle(request.Title);
        InputValidators.EnsureDescriptionLength(request.Description);
        InputValidators.EnsureTaskStatus(request.Status);

        var now = DateTime.UtcNow;
        var task = new TaskItem(
            Id: Guid.NewGuid(),
            UserId: userId,
            Title: request.Title.Trim(),
            Description: request.Description?.Trim(),
            Status: request.Status,
            DueDateUtc: request.DueDateUtc,
            CreatedAtUtc: now,
            UpdatedAtUtc: now);

        await _tasks.AddAsync(task, cancellationToken);
        return ToDto(task);
    }

    public async Task<TaskDto> UpdateAsync(Guid userId, Guid taskId, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        InputValidators.EnsureTaskTitle(request.Title);
        InputValidators.EnsureDescriptionLength(request.Description);
        InputValidators.EnsureTaskStatus(request.Status);

        var existing = await _tasks.GetByIdAsync(taskId, userId, cancellationToken);
        if (existing is null)
            throw new NotFoundException("Task not found.");

        var now = DateTime.UtcNow;
        var updated = existing with
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Status = request.Status,
            DueDateUtc = request.DueDateUtc,
            UpdatedAtUtc = now,
        };

        await _tasks.UpdateAsync(updated, cancellationToken);
        return ToDto(updated);
    }

    public async Task DeleteAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default)
    {
        var deleted = await _tasks.DeleteAsync(taskId, userId, cancellationToken);
        if (!deleted)
            throw new NotFoundException("Task not found.");
    }

    private static TaskDto ToDto(TaskItem t) =>
        new(
            t.Id,
            t.Title,
            t.Description,
            t.Status,
            t.DueDateUtc,
            t.CreatedAtUtc,
            t.UpdatedAtUtc);
}
