using TaskManager.Application.Contracts;

namespace TaskManager.Application.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskDto>> ListAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TaskDto> GetAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default);
    Task<TaskDto> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskDto> UpdateAsync(Guid userId, Guid taskId, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default);
}
