namespace EAD_Backend_Application__.NET.DTOs
{
    public class UserBioDetailsDTO
    {
        public required string Bio { get; set; }
        public required string BusinessName { get; set; }
        public string? BusinessLicenseNumber { get; set; }
        public string? PreferredPaymentMethod { get; set; }
    }
}
