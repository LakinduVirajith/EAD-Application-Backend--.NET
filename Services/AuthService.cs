using EAD_Backend_Application__.NET.Models;
using EAD_Backend_Application__.NET.Services;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ASP.NET___CRUD.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;

        public AuthService(UserManager<ApplicationUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        // REGISTER A NEW USER AND ASSIGN A ROLE
        public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
        {
            // CHECK IF THE EMAIL IS ALREADY REGISTERED
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                // HANDLE THE CASE WHERE THE EMAIL IS ALREADY REGISTERED
                return IdentityResult.Failed(new IdentityError { Description = "Email is already in use." });
            }

            // CREATE A NEW APPLICATIONUSER INSTANCE
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role
            };

            // CHECK IF THE ROLE IS ADMIN AND ALLOW ONLY BASIC FIELDS
            if (model.Role.Equals("Admin") || model.Role.Equals("CSR"))
            {
                user.IsActive = true;
            }
            else if (model.Role.Equals("Vendor") || model.Role.Equals("Customer"))
            {
                // VENDOR ROLE OR CUSTOMER ROLE
                user.DateOfBirth = model.DateOfBirth;
                user.Gender = model.Gender;
                user.Address = model.Address;
                user.City = model.City;
                user.State = model.State;
                user.PostalCode = model.PostalCode;
                user.IsActive = false;
            }
            
            if (model.Role.Equals("Vendor"))
            {
                // VENDOR ROLE: ALLOW ALL FIELDS
                user.Bio = model.Bio;
                user.BusinessName = model.BusinessName;
                user.BusinessLicenseNumber = model.BusinessLicenseNumber;
                user.PreferredPaymentMethod = model.PreferredPaymentMethod;
            }
            

            // CREATE USER WITH PASSWORD
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded && !string.IsNullOrWhiteSpace(model.Role))
            {
                // ASSIGN THE ROLE TO THE USER IF IT'S PROVIDED
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            return result; // RETURN RESULT OF USER CREATION
        }

        // AUTHENTICATE A USER AND RETURN A JWT TOKEN
        public async Task<(string? token, string? refreshToken)> AuthenticateUserAsync(LoginModel model)
        {
            // FIND THE USER BY EMAIL
            var user = await _userManager.FindByEmailAsync(model.Email);

            // CHECK IF THE USER EXISTS AND THE PASSWORD IS CORRECT
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
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
        private async Task<ApplicationUser?> ValidateRefreshToken(string refreshToken)
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
