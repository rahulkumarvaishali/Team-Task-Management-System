using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Auth;
using TaskManagement.API.Interfaces;
using TaskManagement.API.Models;
using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<AuthResponseDto?> RegisterAsync(
            RegisterDto registerDto)
        {
            // Check whether email already exists
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == registerDto.Email);

            if (emailExists)
            {
                return null;
            }

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email.ToLower().Trim(),
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            };

            // Never save the plain-text password
            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                registerDto.Password
            );

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return CreateAuthResponse(user);
        }

        public async Task<AuthResponseDto?> LoginAsync(
            LoginDto loginDto)
        {
            var email = loginDto.Email.ToLower().Trim();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                loginDto.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return CreateAuthResponse(user);
        }

        private AuthResponseDto CreateAuthResponse(User user)
        {
            var expiryMinutes = _configuration.GetValue<int>("Jwt:ExpiryMinutes");

            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = GenerateJwtToken(user, expiresAt);

            return new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                ExpiresAt = expiresAt
            };
        }

        private string GenerateJwtToken(User user, DateTime expiresAt)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");

            var claims = new List<Claim>
            {
                new Claim( ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim( ClaimTypes.Name, user.FullName ),
                new Claim( ClaimTypes.Email, user.Email ),
                new Claim( ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
