using TaskManager.Domain;

namespace TaskManager.Application.Dtos;

public sealed class TaskResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? DueDate { get; init; }
    public TaskItemStatus Status { get; init; }
}
