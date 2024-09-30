using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "User Name is required.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
    }
}
