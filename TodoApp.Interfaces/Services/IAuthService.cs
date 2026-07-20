using System;
using System.Collections.Generic;
using System.Text;
using TodoApp.Interfaces.DTOs.Auth;

namespace TodoApp.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
