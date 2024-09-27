using EAD_Backend_Application__.NET.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Services
{
    public interface IUserService
    {
        Task<IActionResult> ActivateUserAsync(string email);
        Task<IActionResult> DeactivateUserAsync(string email);
        Task<IActionResult> UpdateUserEmailAsync(UpdateEmailDTO dto);
        Task<IActionResult> UpdateUserPasswordAsync(UpdatePasswordDTO dto);
        Task<IActionResult> UpdateUserDetailsAsync(UpdateUserDTO dto);
        Task<IActionResult> DeleteUserAsync(string email);
    }
}
