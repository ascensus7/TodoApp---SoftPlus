using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Interfaces.DTOs.Auth;
using TodoApp.Interfaces.Services;

namespace TodoApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        // POST: /api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if(!result.Success)
            {
                return BadRequest(result);
            }
            return StatusCode(StatusCodes.Status201Created, result);

        }
        // POST: /api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if(!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
