﻿using EAD_Backend_Application__.NET.Models;
using Microsoft.AspNetCore.Identity;

namespace EAD_Backend_Application__.NET.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterModel model);
        Task<(string? token, string? refreshToken)> AuthenticateUserAsync(LoginModel model);
        Task<(string? token, string? refreshToken)> RefreshTokenAsync(string refreshToken);
    }
}