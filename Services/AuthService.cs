using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Enums;
using EAD_Backend_Application__.NET.Models;
using EAD_Backend_Application__.NET.Services;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ASP.NET___CRUD.Services
{
    public class AuthService : IAuthService
    {
        // DEPENDENCIES INJECTED THROUGH CONSTRUCTOR
        private readonly UserManager<UserModel> _userManager;
        private readonly TokenService _tokenService;

        // CONSTRUCTOR TO INJECT DEPENDENCIES
        public AuthService(UserManager<UserModel> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDTO dto)
        {
            // CHECK IF THE EMAIL IS ALREADY REGISTERED
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "409", Description = "Email is already in use." });
            }

            // CHECK IF THE USERNAME IS ALREADY REGISTERED
            var existingUserByUsername = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUserByUsername != null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "409", Description = "Username is already in use." });
            }

            // VALIDATE THE ROLE
            var validRoles = Enum.GetNames(typeof(UserRoles));
            if (!validRoles.Contains(dto.Role))
            {
                return IdentityResult.Failed(new IdentityError { Code = "400", Description = "Invalid role provided." });
            }

            // CREATE A NEW APPLICATIONUSER INSTANCE
            var user = new UserModel
            {
                UserName = dto.UserName,
                ProfileImageUrl = dto.ProfileImageUrl,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Role = dto.Role,
                IsActive = true
            };

            // IF VENDOR ROLE OR CUSTOMER
            if (dto.Role.Equals("Vendor") || dto.Role.Equals("Customer"))
            {
                user.IsActive = false;
            }

            try
            {
                // CREATE USER WITH PASSWORD
                var result = await _userManager.CreateAsync(user, dto.Password);

                if (result.Succeeded && !string.IsNullOrWhiteSpace(dto.Role))
                {
                    // ASSIGN THE ROLE TO THE USER IF IT'S PROVIDED
                    await _userManager.AddToRoleAsync(user, dto.Role);
                }

                return result;
            }
            catch (Exception)
            {
                return IdentityResult.Failed(new IdentityError { Code = "500", Description = "User registration failed due to an unexpected error." });
            } 
        }

        public async Task<(string? token, string? refreshToken)> AuthenticateUserAsync(LoginDTO dto)
        {
            // TRY TO FIND THE USER BY USERNAME
            var user = await _userManager.FindByNameAsync(dto.UserName);

            // IF USER NOT FOUND, TRY TO FIND BY EMAIL
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(dto.UserName);
            }

            // CHECK IF THE USER EXISTS AND THE PASSWORD IS CORRECT
            if (user != null && user.IsActive && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                // GET THE ROLES ASSOCIATED WITH THE USER
                var roles = await _userManager.GetRolesAsync(user);

                // GENERATE JWT TOKEN BASED ON USER AND ROLES
                var token = _tokenService.GenerateToken(user, roles);

                // GENERATE A REFRESH TOKEN FOR THE USER
                var refreshToken = _tokenService.GenerateRefreshToken(user);

                // RETURN THE GENERATED TOKEN AND REFRESH TOKEN
                return (token, refreshToken);
            }

            // RETURN NULL IF AUTHENTICATION FAILS
            return (null, null);
        }

        public async Task<(string? token, string? refreshToken)> RefreshTokenAsync(string refreshToken)
        {
            // VALIDATE THE REFRESH TOKEN AND CHECK ITS EXPIRATION
            var user = await ValidateRefreshToken(refreshToken);
            if (user == null)
            {
                return (null, null); // INVALID REFRESH TOKEN
            }

            // GENERATE NEW TOKENS
            var roles = await _userManager.GetRolesAsync(user);
            var newToken = _tokenService.GenerateToken(user, roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken(user); // CREATE A NEW REFRESH TOKEN

            // OPTIONALLY: STORE THE NEW REFRESH TOKEN IN YOUR DATABASE OR CACHE

            return (newToken, newRefreshToken);
        }

        // VALIDATE THE REFRESH TOKEN
        private async Task<UserModel?> ValidateRefreshToken(string refreshToken)
        {
            // DECODING THE TOKEN TO CHECK EXPIRATION
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = tokenHandler.ReadJwtToken(refreshToken);
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    // TOKEN HAS EXPIRED
                    return null;
                }

                // RETRIEVE THE USER'S EMAIL FROM THE TOKEN CLAIMS
                var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (emailClaim == null) return null;

                // FIND THE USER BASED ON THE EMAIL
                var userEmail = emailClaim.Value;
                return await _userManager.FindByEmailAsync(userEmail);
            }
            catch
            {
                return null;
            }
        }
    }
}
