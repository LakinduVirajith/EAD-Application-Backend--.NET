namespace EAD_Backend_Application__.NET.DTOs
{
    public class UpdateUserDTO
    {
        public string? UserName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? NewEmail { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }

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

        public string? Role { get; set; }
        public bool IsActive { get; set; }
    }
}
