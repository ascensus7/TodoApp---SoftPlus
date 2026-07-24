using System.ComponentModel.DataAnnotations;

namespace TodoApp.Interfaces.DTOs.Tasks;

// Represents data sent by the client when updating a task.
public class UpdateTodoItemRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public int? CategoryId { get; set; }
}