using System.ComponentModel.DataAnnotations;

namespace TodoApp.Interfaces.DTOs.Tasks;

public class CreateTodoItemRequest
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Description { get; set; }


    public DateTimeOffset? DueDate { get; set; }


    public int? CategoryId { get; set; }
}