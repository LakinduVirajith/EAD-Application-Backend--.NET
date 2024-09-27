using EAD_Backend_Application__.NET.DTOs;
using Microsoft.AspNetCore.Identity;

namespace EAD_Backend_Application__.NET.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterDTO dto);
        Task<(string? token, string? refreshToken)> AuthenticateUserAsync(LoginDTO dto);
        Task<(string? token, string? refreshToken)> RefreshTokenAsync(string refreshToken);
    }
}
