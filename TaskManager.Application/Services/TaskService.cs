using TaskManager.Application.Abstractions;
using TaskManager.Application.Dtos;
using TaskManager.Application.Exceptions;
using TaskManager.Domain;

namespace TaskManager.Application.Services;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            DueDate = request.DueDate,
            Status = request.Status
        };

        await _repository.AddAsync(task, cancellationToken);
        return ToResponse(task);
    }

    public async Task<IReadOnlyList<TaskResponse>> ListAsync(
        TaskItemStatus? status,
        DateTime? dueDate,
        CancellationToken cancellationToken = default)
    {
        var items = await _repository.ListAsync(status, dueDate, cancellationToken);
        return items.Select(ToResponse).ToList();
    }

    public async Task<TaskResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);
        return ToResponse(task);
    }

    public async Task<TaskResponse> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);

        task.Title = request.Title.Trim();
        task.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        task.DueDate = request.DueDate;
        task.Status = request.Status;

        await _repository.UpdateAsync(task, cancellationToken);
        return ToResponse(task);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);
        await _repository.DeleteAsync(task, cancellationToken);
    }

    private static TaskResponse ToResponse(TaskItem task) =>
        new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status
        };
}
