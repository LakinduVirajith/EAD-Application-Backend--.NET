using EAD_Backend_Application__.NET.Helpers;
using EAD_Backend_Application__.NET.Models;
using EAD_Backend_Application__.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EAD_Backend_Application__.NET.Controllers
{

    [Route("api/v1")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        // POST: api/v1/auth/sign-up
        [HttpPost("auth/sign-up")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var result = await _authService.RegisterUserAsync(model);

            if (result.Succeeded)
            {
                return Ok("User registered successfully!");
            }

            return BadRequest(result.Errors);
        }

        // POST: api/v1/auth/sign-in
        [HttpPost("auth/sign-in")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var (token, refreshToken) = await _authService.AuthenticateUserAsync(model);

            if (token != null)
            {
                return Ok(new { Token = token, RefreshToken = refreshToken });
            }

            return Unauthorized();
        }

        // POST: api/v1/auth/refresh-token
        [HttpPost("auth/refresh-token/{refreshToken}")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            // Validate the refresh token and generate new tokens
            var (newToken, newRefreshToken) = await _authService.RefreshTokenAsync(refreshToken);
            if (newToken != null)
            {
                return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
            }

            return Unauthorized();
        }

        // PUT: api/v1/users/activate/{email}
        [HttpPut("users/activate/{email}")]
        [Authorize(Roles = "Administrator, CSR")]
        public async Task<IActionResult> ActivateUser(string email)
        {
            return await _userService.ActivateUserAsync(email);
        }

        // PUT: api/v1/users/deactivate/{email}
        [HttpPut("users/deactivate/{email}")]
        public async Task<IActionResult> DeactivateUser(string email)
        {
            return await _userService.DeactivateUserAsync(email);
        }

        // PUT: api/v1/users/update/email
        [HttpPut("users/update/email")]
        [Authorize]
        public async Task<IActionResult> UpdateUserEmail(UpdateEmailModel updateEmail)
        {
            return await _userService.UpdateUserEmailAsync(updateEmail);
        }

        // PUT: api/v1/users/update/password
        [HttpPut("users/update/password")]
        [Authorize]
        public async Task<IActionResult> UpdateUserPassword(UpdatePasswordModel updatePassword)
        {
            return await _userService.UpdateUserPasswordAsync(updatePassword);
        }

        // PUT: api/v1/users/update/details
        [HttpPut("users/update/details")]
        [Authorize]
        public async Task<IActionResult> UpdateUserDetails(UpdateUserModel model)
        {
            return await _userService.UpdateUserDetailsAsync(model);
        }

        // DELETE: api/v1/users/delete/{email}
        [HttpDelete("users/delete/{email}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            return await _userService.DeleteUserAsync(email);
        }
    }
}
