
using EAD_Backend_Application__.NET.Models;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Services
{
    public interface IUserService
    {
        Task<IActionResult> ActivateUserAsync(string email);
        Task<IActionResult> DeactivateUserAsync(string email);
        Task<IActionResult> UpdateUserEmailAsync(UpdateEmailModel updateEmail);
        Task<IActionResult> UpdateUserPasswordAsync(UpdatePasswordModel updatePassword);
        Task<IActionResult> UpdateUserDetailsAsync(UpdateUserModel model);
        Task<IActionResult> DeleteUserAsync(string email);
    }
}
