using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class UserBioDetailsDTO
    {
        [Required(ErrorMessage = "Bio is required.")]
        [StringLength(400, MinimumLength = 2, ErrorMessage = "Bio must be between 2 and 100 characters long.")]
        public required string Bio { get; set; }

        [Required(ErrorMessage = "Business Name is required.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Business Name must be between 2 and 40 characters long.")]
        public required string BusinessName { get; set; }

        [StringLength(40, MinimumLength = 2, ErrorMessage = "Business License Number must be between 2 and 40 characters long.")]
        public string? BusinessLicenseNumber { get; set; }

        [StringLength(40, MinimumLength = 2, ErrorMessage = "Preferred Payment Method must be between 2 and 40 characters long.")]
        public string? PreferredPaymentMethod { get; set; }
    }
}
