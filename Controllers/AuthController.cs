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

    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly UserService _userService;

        public AuthController(IAuthService authService, UserService userService)
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
        [HttpPost("users/activate/{email}")]
        [Authorize(Roles = "Administrator, CSR")]
        public async Task<IActionResult> ActivateUser(string email)
        {
            var result = await _userService.ActivateUserAsync(email);

            if (result.Succeeded)
            {
                return Ok("User account activated successfully!");
            }

            return BadRequest(result.Errors);
        }

        // PUT: api/v1/users/deactivate/{email}
        [HttpDelete("users/deactivate/{email}")]
        public async Task<IActionResult> DeactivateUser(string email)
        {
            var result = await _userService.DeactivateUserAsync(email);

            if (result.Succeeded)
            {
                return Ok("User account deactivated successfully!");
            }

            return BadRequest(result.Errors);
        }

        // PUT: api/v1/users/update/email
        [HttpPut("users/update/email")]
        [Authorize]
        public async Task<IActionResult> UpdateUserEmail(UpdateEmail updateEmail)
        {
            var result = await _userService.UpdateUserEmailAsync(model);

            if (result.Succeeded)
            {
                return Ok("User account updated successfully!");
            }

            return BadRequest(result.Errors);
        }

        // PUT: api/v1/users/update/password
        [HttpPut("users/update/password")]
        [Authorize]
        public async Task<IActionResult> UpdateUserPassword(UpdatePassword updatePassword)
        {
            var result = await _userService.UpdateUserPasswordAsync(model);

            if (result.Succeeded)
            {
                return Ok("User account updated successfully!");
            }

            return BadRequest(result.Errors);
        }

        // PUT: api/v1/users/update/details
        [HttpPut("users/update/details")]
        [Authorize]
        public async Task<IActionResult> UpdateUserDetails(RegisterModel model)
        {
            var result = await _userService.UpdateUserDetailsAsync(model);

            if (result.Succeeded)
            {
                return Ok("User account updated successfully!");
            }

            return BadRequest(result.Errors);
        }

        // DELETE: api/v1/users/delete/{email}
        [HttpPut("users/delete/{email}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var result = await _userService.DeleteUserAsync(email);

            if (result.Succeeded)
            {
                return Ok("User account updated successfully!");
            }

            return BadRequest(result.Errors);
        }
    }
}
