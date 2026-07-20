using System;
using System.Collections.Generic;
using System.Text;

namespace TodoApp.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;

    }
}
