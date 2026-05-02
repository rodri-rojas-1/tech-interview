namespace TaskManager.Domain;

public sealed record TaskItem(
    Guid Id,
    Guid UserId,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTime? DueDateUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
