using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApp.DataAccess.Entities;
using TodoApp.Interfaces.DTOs.Auth;
using TodoApp.Interfaces.Services;

namespace TodoApp.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Errors = ["A user with this email already exists."]
                };
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return CreateAuthResponse(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return InvalidLoginResponse();
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                return InvalidLoginResponse();
            }
            return CreateAuthResponse(user);
        }
        private AuthResponse CreateAuthResponse(ApplicationUser user)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT key was not found in configuration.");

            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT issuer was not found in configuration.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT audience was not found in configuration.");

            var expirationMinutes =
                int.TryParse(
                    _configuration["Jwt:ExpirationMinutes"],
                    out var configuredMinutes)
                        ? configuredMinutes
                        : 120;

            var expiresAt =
                DateTimeOffset.UtcNow.AddMinutes(expirationMinutes);

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt.UtcDateTime,
                signingCredentials: credentials);

            var tokenValue =
                new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponse
            {
                Success = true,
                UserId = user.Id,
                Email = user.Email,
                Token = tokenValue,
                Expiration = expiresAt
            };
        }
        private static AuthResponse InvalidLoginResponse()
        {
            return new AuthResponse
            {
                Success = false,
                Errors = ["Invalid email or password."]
            };
        }
    }
}
