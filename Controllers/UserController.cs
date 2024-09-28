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
        public async Task<IActionResult> UpdateUserDetails(UpdateUserDTO dto)
        {
            return await _userService.UpdateUserDetailsAsync(dto);
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
