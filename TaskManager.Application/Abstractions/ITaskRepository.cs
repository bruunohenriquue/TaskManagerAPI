using TaskManager.Domain;

namespace TaskManager.Application.Abstractions;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> ListAsync(
        TaskItemStatus? status,
        DateTime? dueDate,
        CancellationToken cancellationToken = default);

    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);

    Task DeleteAsync(TaskItem task, CancellationToken cancellationToken = default);
}
