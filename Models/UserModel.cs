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
        public required string Role { get; set; }
        [Required]
        public required bool IsActive { get; set; }

        // NAVIGATION PROPERTY FOR PRODUCTS PUBLISHED BY THE VENDOR
        public virtual ICollection<ProductModel> Products { get; set; }
        public virtual ICollection<CartItemModel> CartItems { get; set; }
        public virtual ICollection<OrderModel> Orders { get; set; }
        public virtual ICollection<RankingModel> Rankings { get; set; }
    }
}
