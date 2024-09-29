using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class UserModel : IdentityUser
    {
        // CUSTOMER SPECIFIC FIELDS
        public string? ProfileImageUrl { get; set; }

        public required string DateOfBirth { get; set; }

        public required string Gender { get; set; }

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
        public required string Role { get; set; }
        [Required]
        public required bool IsActive { get; set; }

        // NAVIGATION PROPERTY FOR PRODUCTS PUBLISHED BY THE VENDOR
        public virtual ICollection<ProductModel> Products { get; set; }
        public virtual ICollection<CartItemModel> CartItems { get; set; }
        public virtual ICollection<OrderModel> Orders { get; set; }
    }
}
