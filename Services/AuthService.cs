using EAD_Backend_Application__.NET.DTOs;
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
        private readonly UserManager<UserModel> _userManager;
        private readonly TokenService _tokenService;

        public AuthService(UserManager<UserModel> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        // REGISTER A NEW USER AND ASSIGN A ROLE
        public async Task<IdentityResult> RegisterUserAsync(RegisterDTO dto)
        {
            // CHECK IF THE EMAIL IS ALREADY REGISTERED
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                // HANDLE THE CASE WHERE THE EMAIL IS ALREADY REGISTERED
                return IdentityResult.Failed(new IdentityError { Description = "Email is already in use." });
            }
            // VALIDATE THE ROLE
            if (!dto.Role.Equals("Admin") && !dto.Role.Equals("CSR") &&
                !dto.Role.Equals("Vendor") && !dto.Role.Equals("Customer"))
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid role provided." });
            }

            // CREATE A NEW APPLICATIONUSER INSTANCE
            var user = new UserModel
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role
            };

            // CHECK IF THE ROLE IS ADMIN AND ALLOW ONLY BASIC FIELDS
            if (dto.Role.Equals("Admin") || dto.Role.Equals("CSR"))
            {
                user.IsActive = true;
            }
            else if (dto.Role.Equals("Vendor") || dto.Role.Equals("Customer"))
            {
                // VENDOR ROLE OR CUSTOMER ROLE
                user.DateOfBirth = dto.DateOfBirth;
                user.Gender = dto.Gender;
                user.Address = dto.Address;
                user.City = dto.City;
                user.State = dto.State;
                user.PostalCode = dto.PostalCode;
                user.IsActive = false;
            }
            
            if (dto.Role.Equals("Vendor"))
            {
                // VENDOR ROLE: ALLOW ALL FIELDS
                user.Bio = dto.Bio;
                user.BusinessName = dto.BusinessName;
                user.BusinessLicenseNumber = dto.BusinessLicenseNumber;
                user.PreferredPaymentMethod = dto.PreferredPaymentMethod;
            }
            

            // CREATE USER WITH PASSWORD
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded && !string.IsNullOrWhiteSpace(dto.Role))
            {
                // ASSIGN THE ROLE TO THE USER IF IT'S PROVIDED
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

            return result; // RETURN RESULT OF USER CREATION
        }

        // AUTHENTICATE A USER AND RETURN A JWT TOKEN
        public async Task<(string? token, string? refreshToken)> AuthenticateUserAsync(LoginDTO dto)
        {
            // TRY TO FIND THE USER BY USERNAME
            var user = await _userManager.FindByNameAsync(dto.Email);

            // IF USER NOT FOUND, TRY TO FIND BY EMAIL
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(dto.Email);
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

        // VALIDATE THE REFRESH TOKEN AND GENERATE NEW TOKENS
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
                // HANDLE TOKEN VALIDATION EXCEPTIONS (E.G., INVALID TOKEN)
                return null;
            }
        }
    }
}
