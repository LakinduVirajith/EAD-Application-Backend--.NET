using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class UserModel : IdentityUser
    {
        // CUSTOMER SPECIFIC FIELDS
        public string? ProfileImageUrl { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        // VENDOR SPECIFIC FIELDS
        public string? Bio { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessLicenseNumber { get; set; }
        public string? PreferredPaymentMethod { get; set; }

        // COMMON FIELDS FOR ALL ADMIN, CSR, VENDORS AND CUSTOMER
        [Required]
        public string Role { get; set; } = string.Empty;
        [Required]
        public bool IsActive { get; set; }

        // NAVIGATION PROPERTY FOR PRODUCTS PUBLISHED BY THE VENDOR
        public virtual ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();
        public UserModel()
        {
            
        }
    }
}
