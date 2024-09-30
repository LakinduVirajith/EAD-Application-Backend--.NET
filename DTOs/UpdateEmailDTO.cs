using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class UpdateEmailDTO
    {
        [Required(ErrorMessage = "Current Email is required.")]
        public required string CurrentEmail { get; set; }

        [Required(ErrorMessage = "New Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid New Email format.")]
        public required string NewEmail { get; set; }
    }
}
