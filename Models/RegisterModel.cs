namespace EAD_Backend_Application__.NET.Models
{
    public class RegisterModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        public string? Bio { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessLicenseNumber { get; set; }
        public string? PreferredPaymentMethod { get; set; }

        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public RegisterModel()
        {
            
        }
    }
}
