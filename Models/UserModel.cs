using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class UserModel : IdentityUser
    {
        // CUSTOMER SPECIFIC FIELDS
        [StringLength(40, MinimumLength = 5, ErrorMessage = "Image Uri must be between 5 and 40 characters.")]
        public string? ProfileImageUrl { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Date of Birth must be in 'yyyy-MM-dd' format.")]
        public string? DateOfBirth { get; set; }

        public string? Gender { get; set; }

        [StringLength(100, MinimumLength = 4, ErrorMessage = "Address must be between 4 and 100 characters long.")]
        public string? Address { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "City must be between 2 and 20 characters long.")]
        public string? City { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "State must be between 2 and 20 characters long.")]
        public string? State { get; set; }

        [StringLength(10, MinimumLength = 5, ErrorMessage = "Postal Code must be between 5 and 10 characters long.")]
        public string? PostalCode { get; set; }

        // VENDOR SPECIFIC FIELDS
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Bio must be between 4 and 100 characters long.")]
        public string? Bio { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Business Name must be between 2 and 20 characters long.")]
        public string? BusinessName { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Business License Number must be between 2 and 20 characters long.")]
        public string? BusinessLicenseNumber { get; set; }

        [StringLength(20, MinimumLength = 2, ErrorMessage = "Preferred Payment Method must be between 2 and 20 characters long.")]
        public string? PreferredPaymentMethod { get; set; }

        // COMMON FIELDS FOR ALL ADMIN, CSR, VENDORS AND CUSTOMER
        [Required]
        public string Role { get; set; } = string.Empty;
        [Required]
        public bool IsActive { get; set; }

        // NAVIGATION PROPERTY FOR PRODUCTS PUBLISHED BY THE VENDOR
        public virtual ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();
        public virtual ICollection<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
        public UserModel()
        {
            
        }
    }
}
