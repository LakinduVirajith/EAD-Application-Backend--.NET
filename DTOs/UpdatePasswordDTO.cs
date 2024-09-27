namespace EAD_Backend_Application__.NET.DTOs
{
    public class UpdatePasswordDTO
    {
        public string Email { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

        public UpdatePasswordDTO()
        {

        }
    }
}
