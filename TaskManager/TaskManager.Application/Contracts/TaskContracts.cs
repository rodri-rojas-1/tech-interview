using TaskManager.Domain;

namespace TaskManager.Application.Contracts;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTime? DueDateUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);

public sealed record CreateTaskRequest(string Title, string? Description, TaskItemStatus Status, DateTime? DueDateUtc);

public sealed record UpdateTaskRequest(string Title, string? Description, TaskItemStatus Status, DateTime? DueDateUtc);
