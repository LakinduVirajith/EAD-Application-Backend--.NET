using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class UpdatePasswordDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Current Password is required.")]
        public required string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New Password is required.")]
        [StringLength(20, MinimumLength = 7, ErrorMessage = "New Password must be between 7 and 20 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "New Password must have at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public required string NewPassword { get; set; }
    }
}
