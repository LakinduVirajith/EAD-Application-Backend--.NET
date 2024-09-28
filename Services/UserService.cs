using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EAD_Backend_Application__.NET.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TokenService _tokenService;

        public UserService(UserManager<UserModel> userManager, IHttpContextAccessor httpContextAccessor, TokenService tokenService)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }

        public async Task<IActionResult> ActivateUserAsync(string email)
        {
            // FIND THE USER BY EMAIL
            var user = await _userManager.FindByEmailAsync(email);

            // CHECK IF USER EXISTS
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = $"User with email {email} not found." });
            }

            // CHECK IF THE USER IS ALREADY ACTIVE
            if (user.IsActive)
            {
                return new ConflictObjectResult(new { Status = "Error", Message = "User is already activated." });
            }

            // ACTIVATE THE USER
            user.IsActive = true;

            // UPDATE THE USER'S STATUS IN THE DATABASE
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = $"User with email {email} activated successfully." });
            }

            // HANDLE FAILURE CASE
            return new ConflictObjectResult(new { Status = "Error", Message = "Failed to activate user. Please try again." });
        }

        public async Task<IActionResult> DeactivateUserAsync(string email)
        {
            // FIND THE USER BY EMAIL
            var user = await _userManager.FindByEmailAsync(email);

            // CHECK IF USER EXISTS
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = $"User with email {email} not found." });
            }

            // CHECK IF THE USER IS ALREADY INACTIVE
            if (!user.IsActive)
            {
                return new ConflictObjectResult(new { Status = "Error", Message = "User is already deactivated." });
            }

            // DEACTIVATE THE USER
            user.IsActive = false;

            // UPDATE THE USER'S STATUS IN THE DATABASE
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = $"User with email {email} deactivated successfully." });
            }

            // HANDLE FAILURE CASE
            return new ConflictObjectResult(new { Status = "Error", Message = "Failed to deactivate user. Please try again." });
        }

        public async Task<IActionResult> UpdateUserImageAsync(IFormFile imageFile)
        {
            // GET THE EMAIL FROM THE AUTHENTICATION HEADER
            var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

            // CHECK IF EMAIL IS NULL
            if (string.IsNullOrEmpty(email))
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "User not found. Please ensure you are logged in." });
            }

            // FIND THE USER BY EMAIL
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "User not found. Please ensure you are logged in." });
            }
            return new NotFoundObjectResult(new { Status = "Error", Message = "This feature is currently under development and is not yet implemented." });
        }

        public async Task<IActionResult> UpdateUserEmailAsync(UpdateEmailDTO dto)
        {
            // FIND THE USER BY CURRENT EMAIL
            var user = await _userManager.FindByEmailAsync(dto.CurrentEmail);

            // CHECK IF USER EXISTS
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = $"User with email {dto.CurrentEmail} not found." });
            }

            // CHECK IF THE NEW EMAIL IS ALREADY IN USE
            var existingUserWithNewEmail = await _userManager.FindByEmailAsync(dto.NewEmail);
            if (existingUserWithNewEmail != null)
            {
                return new ConflictObjectResult(new { Status = "Error", Message = $"Email {dto.NewEmail} is already in use." });
            }

            // UPDATE USER EMAIL
            user.Email = dto.NewEmail;

            // UPDATE THE USER'S DETAILS IN THE DATABASE
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = $"User email updated to {dto.NewEmail} successfully." });
            }

            // HANDLE FAILURE CASE
            return new ConflictObjectResult(new { Status = "Error", Message = "Failed to update user email. Please try again." });
        }

        public async Task<IActionResult> UpdateUserPasswordAsync(UpdatePasswordDTO dto)
        {
            // FIND THE USER BY EMAIL
            var user = await _userManager.FindByEmailAsync(dto.Email);

            // CHECK IF USER EXISTS
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = $"User with email {dto.Email} not found." });
            }

            // VERIFY THE CURRENT PASSWORD
            var passwordVerificationResult = await _userManager.CheckPasswordAsync(user, dto.CurrentPassword);
            if (!passwordVerificationResult)
            {
                return new ConflictObjectResult(new { Status = "Error", Message = "Current password is incorrect." });
            }

            // UPDATE THE USER'S PASSWORD
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = "Password updated successfully." });
            }

            // HANDLE FAILURE CASE
            return new ConflictObjectResult(new { Status = "Error", Message = "Failed to update password. Please try again." });
        }

        public async Task<IActionResult> UpdateUserDetailsAsync(UpdateUserDTO dto)
        {
            // FIND THE USER BY EMAIL
            var user = await _userManager.FindByEmailAsync(dto.Email);

            // CHECK IF USER EXISTS
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = $"User with email {dto.Email} not found." });
            }

            // COMMONLY UPDATED FIELDS
            user.UserName = dto.UserName;
            user.PhoneNumber = dto.PhoneNumber;
            user.DateOfBirth = dto.DateOfBirth;
            user.ProfileImageUrl = dto.ProfileImageUrl;
            user.Gender = dto.Gender;
            user.Address = dto.Address;
            user.City = dto.City;
            user.State = dto.State;
            user.PostalCode = dto.PostalCode;

            // UPDATE USER DETAILS BASED ON ROLE
            if (user.Role.Equals("Admin") || user.Role.Equals("CSR"))
            {
                // ADMIN ROLE: UPDATE BASIC FIELDS ONLY
                user.IsActive = dto.IsActive;
                user.Bio = dto.Bio;
                user.BusinessName = dto.BusinessName;
                user.BusinessLicenseNumber = dto.BusinessLicenseNumber;
                user.PreferredPaymentMethod = dto.PreferredPaymentMethod;

                // CHANGE EMAIL
                if (!string.IsNullOrEmpty(dto.NewEmail))
                {
                    var existingUser = await _userManager.FindByEmailAsync(dto.NewEmail);
                    if (existingUser != null)
                    {
                        return new ConflictObjectResult(new { Status = "Error", Message = "New Email is already in use by another user." });
                    }

                    user.Email = dto.NewEmail;
                }

                // CHANGE PASSWORD
                if (!string.IsNullOrEmpty(dto.Password))
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetResult = await _userManager.ResetPasswordAsync(user, resetToken, dto.Password);
                    if (!passwordResetResult.Succeeded)
                    {
                        return new ConflictObjectResult(new { Status = "Error", Message = "Failed to update password." });
                    }
                }
            }
            else if (user.Role.Equals("Vendor"))
            {
                // VENDOR ROLE: UPDATE ALL FIELDS
                user.Bio = dto.Bio;
                user.BusinessName = dto.BusinessName;
                user.BusinessLicenseNumber = dto.BusinessLicenseNumber;
                user.PreferredPaymentMethod = dto.PreferredPaymentMethod;
            }

            // UPDATE THE USER IN THE DATABASE
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = $"User with email {dto.Email} updated successfully." });
            }

            // HANDLE FAILURE CASE
            return new ConflictObjectResult(new { Status = "Error", Message = "Failed to update user details. Please try again." });
        }

        public async Task<IActionResult> DeleteUserAsync(string email)
        {
            // FIND THE USER BY EMAIL
            var user = await _userManager.FindByEmailAsync(email);

            // CHECK IF USER EXISTS
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = $"User with email {email} not found." });
            }

            // DELETE THE USER
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = $"User with email {email} deleted successfully." });
            }

            // HANDLE FAILURE CASE
            return new ConflictObjectResult(new { Status = "Error", Message = "Failed to delete user. Please try again." });
        }
    }
    }
