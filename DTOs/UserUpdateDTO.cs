namespace EAD_Backend_Application__.NET.DTOs
{
    public class UserUpdateDTO
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? NewEmail { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; }
        public required string DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }

        public string? Bio { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessLicenseNumber { get; set; }
        public string? PreferredPaymentMethod { get; set; }

        public required string Role { get; set; }
        public required bool IsActive { get; set; }
    }
}
