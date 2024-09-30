using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class UserShippingDetailsDTO
    {
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Address must be between 2 and 200 characters long.")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "City must be between 2 and 40 characters long.")]
        public required string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "State must be between 2 and 40 characters long.")]
        public required string State { get; set; }

        [Required(ErrorMessage = "Postal Code is required.")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Postal Code must be between 5 and 10 characters long.")]
        public required string PostalCode { get; set; }
    }
}
