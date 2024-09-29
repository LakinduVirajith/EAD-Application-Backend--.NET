using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class RegisterDTO
    {
        public string UserName { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;

        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
        [StringLength(15, MinimumLength = 7, ErrorMessage = "Phone number must be between 7 and 15 digits long.")]
        
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(40, MinimumLength = 5, ErrorMessage = "Image Uri must be between 5 and 40 characters.")]
        public string? ProfileImageUrl { get; set; }

        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date of Birth must be in 'yyyy-MM-dd' format.")]
        public string DateOfBirth { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
