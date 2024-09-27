namespace EAD_Backend_Application__.NET.Models
{
    public class UpdatePasswordModel
    {
        public string Email { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

        public UpdatePasswordModel()
        {
            
        }
    }
}
