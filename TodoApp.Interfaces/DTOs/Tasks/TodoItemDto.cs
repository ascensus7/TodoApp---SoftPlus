namespace TodoApp.Interfaces.DTOs.Tasks;

public class TodoItemDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }
}