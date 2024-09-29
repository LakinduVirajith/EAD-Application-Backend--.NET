using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EAD_Backend_Application__.NET.Controllers
{
    [Route("api/v1/user")]
    [ApiController]
    public class UserController
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>Allows an Admin or CSR to activate a user account by their email address.</summary>
        // PUT: api/v1/user/activate/{email}
        [HttpPut("activate/{email}")]
        [Authorize(Roles = "Admin, CSR")]
        public async Task<IActionResult> ActivateUser(string email)
        {
            return await _userService.ActivateUserAsync(email);
        }

        /// <summary>Allows anyone to deactivate a user account by their email address.</summary>
        // PUT: api/v1/user/deactivate/{email}
        [HttpPut("deactivate/{email}")]
        public async Task<IActionResult> DeactivateUser(string email)
        {
            return await _userService.DeactivateUserAsync(email);
        }

        /// <summary>Enables users to update their profile image.</summary>
        // PUT: api/v1/user/image
        [HttpPut("image")]
        [Authorize]
        public async Task<IActionResult> UpdateUserImage([FromForm] FileUploadDTO dto)
        {
            if (dto == null || dto.File.Length == 0)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Please provide a valid image file." });
            }
            return await _userService.UpdateUserImageAsync(dto.File);
        }

        /// <summary>Allows an authenticated user to update their email address.</summary>
        // PUT: api/v1/user/update/email
        [HttpPut("update/email")]
        [Authorize]
        public async Task<IActionResult> UpdateUserEmail(UpdateEmailDTO dto)
        {
            return await _userService.UpdateUserEmailAsync(dto);
        }

        /// <summary>Allows an authenticated user to update their password.</summary>
        // PUT: api/v1/user/update/password
        [HttpPut("update/password")]
        [Authorize]
        public async Task<IActionResult> UpdateUserPassword(UpdatePasswordDTO dto)
        {
            return await _userService.UpdateUserPasswordAsync(dto);
        }

        /// <summary>Allows an authenticated user to update their personal details.</summary>
        // PUT: api/v1/user/update/details
        [HttpPut("update/details")]
        [Authorize]
        public async Task<IActionResult> UpdateUserDetails(UserUpdateDTO dto)
        {
            return await _userService.UpdateUserDetailsAsync(dto);
        }

        /// <summary>Allows an authenticated user to update their shipping details.</summary>
        // PUT: api/v1/user/update/shipping
        [HttpPut("update/shipping")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateUserShipping(UserShippingDetailsDTO dto)
        {
            return await _userService.UpdateUserShippingAsync(dto);
        }

        /// <summary>Allows an authenticated user to update their bio details.</summary>
        // PUT: api/v1/user/update/bio
        [HttpPut("update/bio")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateUserBio(UserBioDetailsDTO dto)
        {
            return await _userService.UpdateUserBioAsync(dto);
        }

        /// <summary>Retrieves the authenticated user's profile details.</summary>
        // GET: api/v1/user/details
        [HttpGet("details")]
        [Authorize]
        public async Task<ActionResult<UserGetDTO>> GetUserDetails()
        {
            return await _userService.GetUserDetailsAsync();
        }

        /// <summary>Retrieves user details for admin or CSR users with pagination support.</summary>
        // GET: api/v1/user/details/admin
        [HttpGet("details/admin")]
        [Authorize(Roles = "Admin, CSR")]
        public async Task<ActionResult<IEnumerable<UserGetDTO>>> GetUserDetailsAdmin(int pageNumber, int pageSize)
        {
            return await _userService.GetUserDetailsAdminAsync(pageNumber, pageSize);
        }

        /// <summary>Retrieves user details based on the provided email address for admin or CSR users.</summary>
        // GET: api/v1/user/details/{email}
        [HttpGet("details/{email}")]
        [Authorize(Roles = "Admin, CSR")]
        public async Task<ActionResult<UserUpdateDTO>> GetUserDetailsByEmail(string email)
        {
            return await _userService.GetUserDetailsByEmailAsync(email);
        }

        /// <summary>Allows an Admin to delete a user account by their email address.</summary>
        // DELETE: api/v1/user/delete/{email}
        [HttpDelete("delete/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            return await _userService.DeleteUserAsync(email);
        }
    }
}
