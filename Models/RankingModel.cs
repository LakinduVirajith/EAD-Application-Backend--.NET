using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class RankingModel
    {
        [Key]
        public string RankingId { get; set; } = Guid.NewGuid().ToString();
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
        public string CustomerId { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;

        // FOREIGN KEY TO VENDOR
        [Required]
        public string VendorId { get; set; } = string.Empty;
        public virtual UserModel Vendor { get; set; }
    }
}
