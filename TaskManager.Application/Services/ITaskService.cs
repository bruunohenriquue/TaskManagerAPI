using TaskManager.Application.Dtos;
using TaskManager.Domain;

namespace TaskManager.Application.Services;

public interface ITaskService
{
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskResponse>> ListAsync(
        TaskItemStatus? status,
        DateTime? dueDate,
        CancellationToken cancellationToken = default);

    Task<TaskResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
