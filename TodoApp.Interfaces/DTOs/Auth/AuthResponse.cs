using System;
using System.Collections.Generic;
using System.Text;

namespace TodoApp.Interfaces.DTOs.Auth
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public DateTimeOffset? Expiration { get; set; }
        public IEnumerable<string> Errors { get; set; } = [];
    }
}
