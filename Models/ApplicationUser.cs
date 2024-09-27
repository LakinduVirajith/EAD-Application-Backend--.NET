using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EAD_Backend_Application__.NET.Models
{
    public class ApplicationUser : IdentityUser
    {
        // CUSTOMER SPECIFIC FIELDS
        public string? ProfileImageUrl { get; set; }
        public DateOnly DateOfBirth { get; set; }
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

        // COMMON FIELDS FOR BOTH CUSTOMER, VENDORS AND ADMIN
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public ApplicationUser()
        {
            
        }
    }
}
