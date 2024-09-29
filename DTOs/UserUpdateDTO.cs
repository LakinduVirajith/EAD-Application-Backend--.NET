using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class UserUpdateDTO
    {
        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "User Name must be between 2 and 40 characters.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public string? Email { get; set; }

        [EmailAddress(ErrorMessage = "Invalid New Email format.")]
        public string? NewEmail { get; set; }

        [StringLength(20, MinimumLength = 7, ErrorMessage = "Password must be between 7 and 20 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
        [StringLength(15, MinimumLength = 7, ErrorMessage = "Phone number must be between 7 and 15 digits long.")]
        public required string PhoneNumber { get; set; }

        [StringLength(100, MinimumLength = 5, ErrorMessage = "Image Uri must be between 5 and 100 characters.")]
        public string? ProfileImageUrl { get; set; }

        [Required(ErrorMessage = "Date Of Birth is required.")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date of Birth must be in 'yyyy-MM-dd' format.")]
        public required string DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public required string Gender { get; set; }

        [StringLength(200, MinimumLength = 2, ErrorMessage = "Address must be between 2 and 200 characters long.")]
        public string? Address { get; set; }

        [StringLength(40, MinimumLength = 2, ErrorMessage = "City must be between 2 and 40 characters long.")]
        public string? City { get; set; }

        [StringLength(40, MinimumLength = 2, ErrorMessage = "State must be between 2 and 40 characters long.")]
        public string? State { get; set; }

        [StringLength(10, MinimumLength = 5, ErrorMessage = "Postal Code must be between 5 and 10 characters long.")]
        public string? PostalCode { get; set; }

        [StringLength(400, MinimumLength = 2, ErrorMessage = "Bio must be between 2 and 400 characters long.")]
        public string? Bio { get; set; }

        [StringLength(40, MinimumLength = 2, ErrorMessage = "Business Name must be between 2 and 40 characters long.")]
        public string? BusinessName { get; set; }

        [StringLength(40, MinimumLength = 2, ErrorMessage = "Business License Number must be between 2 and 40 characters long.")]
        public string? BusinessLicenseNumber { get; set; }

        [StringLength(40, MinimumLength = 2, ErrorMessage = "Preferred Payment Method must be between 2 and 40 characters long.")]
        public string? PreferredPaymentMethod { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public required string Role { get; set; }
        public bool IsActive { get; set; }
    }
}
