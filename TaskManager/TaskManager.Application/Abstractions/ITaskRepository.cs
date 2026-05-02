using TaskManager.Domain;

namespace TaskManager.Application.Abstractions;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdAsync(Guid taskId, Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid taskId, Guid userId, CancellationToken cancellationToken = default);
}
