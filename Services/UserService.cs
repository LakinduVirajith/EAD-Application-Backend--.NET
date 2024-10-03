using Azure.Storage.Blobs;
using EAD_Backend_Application__.NET.Data;
using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Enums;
using EAD_Backend_Application__.NET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EAD_Backend_Application__.NET.Services
{
    public class UserService : IUserService
    {
        // DEPENDENCIES INJECTED THROUGH CONSTRUCTOR
        private readonly UserManager<UserModel> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly TokenService _tokenService;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _blobContainerName;

        // CONSTRUCTOR TO INJECT DEPENDENCIES
        public UserService(UserManager<UserModel> userManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, TokenService tokenService, IConfiguration configuration)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _tokenService = tokenService;

            _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"));
            _blobContainerName = configuration.GetValue<string>("BlobContainerNames:ForAccount")
                                ?? throw new ArgumentNullException(nameof(_blobContainerName), "Blob container name for products is not configured.");
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

            try
            {
                // CREATE A UNIQUE FILENAME USING A GUID
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                // GET A REFERENCE TO THE BLOB CONTAINER
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);

                // ENSURE THE CONTAINER EXISTS
                await blobContainerClient.CreateIfNotExistsAsync();

                // GET A REFERENCE TO THE BLOB (FILE)
                var blobClient = blobContainerClient.GetBlobClient(fileName);

                // UPLOAD THE FILE TO THE BLOB
                using (var stream = imageFile.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                // GENERATE THE URI OF THE UPLOADED IMAGE
                var imageUri = blobClient.Uri.ToString();

                // RETURN THE URI TO THE FRONT-END
                return new OkObjectResult(new { Status = "Success", ImageUrl = imageUri });
            }
            catch (Exception)
            {
                // RETURN ERROR RESPONSE IF SOMETHING FAILS
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while saving the user image. Please try again later." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
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

        public async Task<IActionResult> UpdateUserDetailsAsync(UserUpdateDTO dto)
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

            // COMMONLY UPDATED FIELDS
            user.UserName = dto.UserName;
            user.PhoneNumber = dto.PhoneNumber;
            user.DateOfBirth = dto.DateOfBirth;
            user.ProfileImageUrl = dto.ProfileImageUrl;
            user.Gender = dto.Gender;

            // UPDATE USER DETAILS BASED ON ROLE
            if (user.Role.Equals("Admin") || user.Role.Equals("CSR"))
            {
                // ADMIN ROLE: UPDATE BASIC FIELDS ONLY
                if (dto.Role.Equals("Customer"))
                {
                    if (dto.Address == null) {
                        return new BadRequestObjectResult(new { Status = "Error", Message = "Address is required." });
                    }
                    if (dto.City == null)
                    {
                        return new BadRequestObjectResult(new { Status = "Error", Message = "City is required." });
                    }
                    if (dto.State == null)
                    {
                        return new BadRequestObjectResult(new { Status = "Error", Message = "State is required." });
                    }
                    if (dto.PostalCode == null)
                    {
                        return new BadRequestObjectResult(new { Status = "Error", Message = "PostalCode is required." });
                    }

                    user.Address = dto.Address;
                    user.City = dto.City;
                    user.State = dto.State;
                    user.PostalCode = dto.PostalCode;
                }else if (dto.Role.Equals("Vendor"))
                {
                    if (dto.Bio == null)
                    {
                        return new BadRequestObjectResult(new { Status = "Error", Message = "Bio is required." });
                    }
                    if (dto.BusinessName == null)
                    {
                        return new BadRequestObjectResult(new { Status = "Error", Message = "Business Name is required." });
                    }
                    if (dto.BusinessLicenseNumber != null)
                    {
                        user.BusinessLicenseNumber = dto.BusinessLicenseNumber;
                    }
                    if (dto.PreferredPaymentMethod != null)
                    {
                        user.PreferredPaymentMethod = dto.PreferredPaymentMethod;
                    }

                    user.Bio = dto.Bio;
                    user.BusinessName = dto.BusinessName;
                }
                
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

            // UPDATE THE USER IN THE DATABASE
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = $"User with email {dto.Email} updated successfully." });
            }

            // HANDLE FAILURE CASE
            return new ConflictObjectResult(new { Status = "Error", Message = "Failed to update user details. Please try again." });
        }

        public async Task<IActionResult> CheckUserShippingAsync()
        {
            // GET THE EMAIL FROM AUTHENTICATION HEADER
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

            // CHECK IF USER HAS COMPLETE SHIPPING DETAILS
            bool hasShippingDetails = HasCompleteShippingDetails(user);

            return new OkObjectResult(new
            {
                Status = "Success",
                Message = hasShippingDetails ? "User has shipping details." : "User does not have shipping details.",
                Body = hasShippingDetails
            });
        }

        public async Task<ActionResult<UserShippingDetailsDTO>> GetUserShippingAsync()
        {
            // GET THE EMAIL FROM AUTHENTICATION HEADER
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

            var userShippingDetails = new UserShippingDetailsDTO
            {
                Address = user.Address ?? string.Empty,
                City = user.City ?? string.Empty,
                State = user.State ?? string.Empty,
                PostalCode = user.PostalCode ?? string.Empty,
            };

            // RETURN USER DETAILS
            return new OkObjectResult(userShippingDetails);
        }

        public async Task<IActionResult> UpdateUserShippingAsync(UserShippingDetailsDTO dto)
        {
            // GET THE EMAIL FROM AUTHENTICATION HEADER
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

            // UPDATE SHIPPING DETAILS
            user.Address = dto.Address;
            user.City = dto.City;
            user.State = dto.State;
            user.PostalCode = dto.PostalCode;

            // UPDATE IN DATABASE
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = "Shipping details updated successfully." });
            }

            return new ConflictObjectResult(new { Status = "Error", Message = "Failed to update shipping details. Please try again." });
        }

        public async Task<IActionResult> UpdateUserBioAsync(UserBioDetailsDTO dto)
        {
            // GET THE EMAIL FROM AUTHENTICATION HEADER
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

            // UPDATE BIO DETAILS
            user.Bio = dto.Bio;
            user.BusinessName = dto.BusinessName;
            if(dto.BusinessLicenseNumber != null)
            {
                user.BusinessLicenseNumber = dto.BusinessLicenseNumber;
            }
            if (dto.PreferredPaymentMethod != null) {
                if (Enum.TryParse(typeof(PaymentMethod), dto.PreferredPaymentMethod, true, out var validPaymentMethod))
                {
                    user.PreferredPaymentMethod = dto.PreferredPaymentMethod;
                }
                else
                {
                    return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid payment method." });
                }
            }

            // UPDATE IN DATABASE
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new OkObjectResult(new { Status = "Success", Message = "Vendor bio updated successfully." });
            }

            return new ConflictObjectResult(new { Status = "Error", Message = "Failed to update vendor bio. Please try again." });
        }

        public async Task<ActionResult<UserGetDTO>> GetUserDetailsAsync()
        {
            // GET THE EMAIL FROM AUTHENTICATION HEADER
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

            var userDetails = new UserGetDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender
            };

            // RETURN USER DETAILS
            return new OkObjectResult(userDetails);
        }

        public async Task<ActionResult<IEnumerable<UserGetDTO>>> GetUserDetailsAdminAsync(string userRole, int pageNumber, int pageSize)
        {
            // VALIDATE PAGINATION PARAMETERS
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid pagination parameters." });
            }

            // VALIDATE THE ROLE
            var validRoles = Enum.GetNames(typeof(UserRoles));
            if (!validRoles.Contains(userRole))
            {
                return new BadRequestObjectResult(new IdentityError { Code = "Error", Description = "Invalid role provided." });
            }

            // GET TOTAL USERS COUNT
            var totalUsers = await _userManager.Users.CountAsync(u => u.Role == userRole);

            // FETCH ALL USERS FROM USER MANAGER
            var users = _userManager.Users
                .Where(u => u.Role == userRole)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // MAP USERS TO DTOs
            var userDetailsList = users.Select(user => new UserGetDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender
            }).ToList();

            // CREATE PAGINATION RESPONSE
            var paginationResponse = new
            {
                Status = "Success",
                TotalCount = totalUsers,
                PageNumber = pageNumber,
                PageSize = pageSize,
                UserList = userDetailsList
            };

            // RETURN LIST OF USER DETAILS
            return new OkObjectResult(new { Status = "Success", Body = paginationResponse });
        }

        public async Task<ActionResult<UserUpdateDTO>> GetUserDetailsByEmailAsync(string email)
        {
            // FIND THE USER BY EMAIL
            var user = await _userManager.FindByEmailAsync(email);

            // CHECK IF USER EXISTS
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = $"User with email {email} not found." });
            }

            // MAP USER TO DTO
            var userDetails = new UserUpdateDTO
            {
                UserName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : "",
                Email = user.Email,
                PhoneNumber = !string.IsNullOrEmpty(user.PhoneNumber) ? user.PhoneNumber : "",
                ProfileImageUrl = user.ProfileImageUrl,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Role = user.Role,
                IsActive = user.IsActive
            };

            if (user.Role.Equals("Customer"))
            {
                userDetails.Address = user.Address;
                userDetails.City = user.City;
                userDetails.State = user.State;
                userDetails.PostalCode = user.PostalCode;
            }
            if (user.Role.Equals("Vendor"))
            {
                userDetails.Bio = user.Bio;
                userDetails.BusinessName = user.BusinessName;
                if (user.BusinessLicenseNumber != null)
                {
                    userDetails.BusinessLicenseNumber = user.BusinessLicenseNumber;
                }
                if (user.PreferredPaymentMethod != null)
                {
                    userDetails.PreferredPaymentMethod = user.PreferredPaymentMethod;
                }
            }

            // RETURN USER DETAILS
            return new OkObjectResult(userDetails);
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

            // CHECK IF USER HAS ANY RELATED PRODUCTS
            var products = await _context.Products.Where(p => p.VendorId == user.Id).ToListAsync();
            if (products.Any())
            {
                return new ConflictObjectResult(new { Status = "Error", Message = "User cannot be deleted because they have associated products." });
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

        // HELPER METHOD TO CHECK COMPLETE SHIPPING DETAILS
        private bool HasCompleteShippingDetails(UserModel user)
        {
            return !string.IsNullOrEmpty(user.Address) &&
                   !string.IsNullOrEmpty(user.City) &&
                   !string.IsNullOrEmpty(user.State) &&
                   !string.IsNullOrEmpty(user.PostalCode);
        }
    }
}
