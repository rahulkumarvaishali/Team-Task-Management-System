using TaskManagement.API.DTOs.Auth;

namespace TaskManagement.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);

        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    }
}
