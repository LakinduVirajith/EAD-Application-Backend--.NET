using EAD_Backend_Application__.NET.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenService _tokenService;

        public UserService(UserManager<ApplicationUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
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

        public async Task<IActionResult> UpdateUserEmailAsync(UpdateEmailModel updateEmail)
        {
            // FIND THE USER BY CURRENT EMAIL
            var user = await _userManager.FindByEmailAsync(updateEmail.CurrentEmail);

            // CHECK IF USER EXISTS
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = $"User with email {updateEmail.CurrentEmail} not found." });
            }

            // CHECK IF THE NEW EMAIL IS ALREADY IN USE
            var existingUserWithNewEmail = await _userManager.FindByEmailAsync(updateEmail.NewEmail);
            if (existingUserWithNewEmail != null)
            {
                return new ConflictObjectResult(new { Status = "Error", Message = $"Email {updateEmail.NewEmail} is already in use." });
            }

            // UPDATE USER EMAIL
            user.Email = updateEmail.NewEmail;

            // UPDATE THE USER'S DETAILS IN THE DATABASE
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = $"User email updated to {updateEmail.NewEmail} successfully." });
            }

            // HANDLE FAILURE CASE
            return new ConflictObjectResult(new { Status = "Error", Message = "Failed to update user email. Please try again." });
        }

        public async Task<IActionResult> UpdateUserPasswordAsync(UpdatePasswordModel updatePassword)
        {
            // FIND THE USER BY EMAIL
            var user = await _userManager.FindByEmailAsync(updatePassword.Email);

            // CHECK IF USER EXISTS
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = $"User with email {updatePassword.Email} not found." });
            }

            // VERIFY THE CURRENT PASSWORD
            var passwordVerificationResult = await _userManager.CheckPasswordAsync(user, updatePassword.CurrentPassword);
            if (!passwordVerificationResult)
            {
                return new ConflictObjectResult(new { Status = "Error", Message = "Current password is incorrect." });
            }

            // UPDATE THE USER'S PASSWORD
            var result = await _userManager.ChangePasswordAsync(user, updatePassword.CurrentPassword, updatePassword.NewPassword);

            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = "Password updated successfully." });
            }

            // HANDLE FAILURE CASE
            return new ConflictObjectResult(new { Status = "Error", Message = "Failed to update password. Please try again." });
        }

        public async Task<IActionResult> UpdateUserDetailsAsync(UpdateUserModel model)
        {
            // FIND THE USER BY EMAIL
            var user = await _userManager.FindByEmailAsync(model.Email);

            // CHECK IF USER EXISTS
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = $"User with email {model.Email} not found." });
            }

            // COMMONLY UPDATED FIELDS
            user.UserName = model.UserName;
            user.PhoneNumber = model.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;
            user.Address = model.Address;
            user.City = model.City;
            user.State = model.State;
            user.PostalCode = model.PostalCode;

            // UPDATE USER DETAILS BASED ON ROLE
            if (user.Role.Equals("Admin") || user.Role.Equals("CSR"))
            {
                // ADMIN ROLE: UPDATE BASIC FIELDS ONLY
                user.IsActive = model.IsActive;
                user.Bio = model.Bio;
                user.BusinessName = model.BusinessName;
                user.BusinessLicenseNumber = model.BusinessLicenseNumber;
                user.PreferredPaymentMethod = model.PreferredPaymentMethod;

                // CHANGE EMAIL
                if (!string.IsNullOrEmpty(model.NewEmail))
                {
                    var existingUser = await _userManager.FindByEmailAsync(model.NewEmail);
                    if (existingUser != null)
                    {
                        return new ConflictObjectResult(new { Status = "Error", Message = "New Email is already in use by another user." });
                    }

                    user.Email = model.NewEmail;
                }

                // CHANGE PASSWORD
                if (!string.IsNullOrEmpty(model.Password))
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetResult = await _userManager.ResetPasswordAsync(user, resetToken, model.Password);
                    if (!passwordResetResult.Succeeded)
                    {
                        return new ConflictObjectResult(new { Status = "Error", Message = "Failed to update password." });
                    }
                }
            }
            else if (user.Role.Equals("Vendor"))
            {
                // VENDOR ROLE: UPDATE ALL FIELDS
                user.Bio = model.Bio;
                user.BusinessName = model.BusinessName;
                user.BusinessLicenseNumber = model.BusinessLicenseNumber;
                user.PreferredPaymentMethod = model.PreferredPaymentMethod;
            }

            // UPDATE THE USER IN THE DATABASE
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = $"User with email {model.Email} updated successfully." });
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
