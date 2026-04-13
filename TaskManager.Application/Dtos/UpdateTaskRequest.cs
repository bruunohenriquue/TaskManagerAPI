using System.ComponentModel.DataAnnotations;
using TaskManager.Domain;

namespace TaskManager.Application.Dtos;

public sealed class UpdateTaskRequest
{
    [Required(ErrorMessage = "Título é obrigatório.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public TaskItemStatus Status { get; set; }
}
