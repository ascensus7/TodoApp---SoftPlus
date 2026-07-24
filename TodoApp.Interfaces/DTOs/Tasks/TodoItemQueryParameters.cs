using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TodoApp.Interfaces.DTOs.Tasks
{
    public class TodoItemQueryParameters
    {
        [Range(1,int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        [MaxLength(100)]
        public string? Search { get; set; }

        [Range(1, int.MaxValue)]
        public int? CategoryId { get; set; }

        public bool? IsCompleted { get; set; }
    }
}
