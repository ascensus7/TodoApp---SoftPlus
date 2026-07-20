using System;
using System.Collections.Generic;
using System.Text;

namespace TodoApp.Domain.Entities
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DueDate { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int? CategoryId { get; set; }

        public Category? Category { get; set; } //Nav property to Category entity
    }
}
