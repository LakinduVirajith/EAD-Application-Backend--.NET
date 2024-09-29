namespace EAD_Backend_Application__.NET.DTOs
{
    public class UserGetDTO
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; }
        public required string DateOfBirth { get; set; }
        public required string Gender { get; set; }
    }
}
