using EAD_Backend_Application__.NET.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Services
{
    public interface IUserService
    {
        Task<IActionResult> ActivateUserAsync(string email);
        Task<IActionResult> DeactivateUserAsync(string email);
        Task<IActionResult> UpdateUserImageAsync(IFormFile imageFile);
        Task<IActionResult> UpdateUserEmailAsync(UpdateEmailDTO dto);
        Task<IActionResult> UpdateUserPasswordAsync(UpdatePasswordDTO dto);
        Task<IActionResult> UpdateUserDetailsAsync(UserUpdateDTO dto);
        Task<IActionResult> CheckUserShippingAsync();
        Task<ActionResult<UserShippingDetailsDTO>> GetUserShippingAsync();
        Task<IActionResult> UpdateUserShippingAsync(UserShippingDetailsDTO dto);
        Task<IActionResult> UpdateUserBioAsync(UserBioDetailsDTO dto);
        Task<ActionResult<UserGetDTO>> GetUserDetailsAsync();
        Task<ActionResult<IEnumerable<UserGetDTO>>> GetUserDetailsAdminAsync(string userRole, int pageNumber, int pageSize);
        Task<ActionResult<UserUpdateDTO>> GetUserDetailsByEmailAsync(string email);
        Task<IActionResult> DeleteUserAsync(string email);
    }
}
