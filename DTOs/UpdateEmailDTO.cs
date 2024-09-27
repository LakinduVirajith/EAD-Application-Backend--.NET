namespace EAD_Backend_Application__.NET.DTOs
{
    public class UpdateEmailDTO
    {
        public string CurrentEmail { get; set; } = string.Empty;
        public string NewEmail { get; set; } = string.Empty;

        public UpdateEmailDTO()
        {

        }
    }
}
