namespace TaskManager.Application.Exceptions;

public sealed class TaskNotFoundException : Exception
{
    public TaskNotFoundException(Guid taskId)
        : base($"Tarefa com id '{taskId}' não foi encontrada.")
    {
        TaskId = taskId;
    }

    public Guid TaskId { get; }
}
