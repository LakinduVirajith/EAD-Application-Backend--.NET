using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class RankingDetailsDTO
    {
        public required string VendorId { get; set; }
        public required string Comment { get; set; }
        public required int Rating { get; set; } = 0;
        public required string CustomerId { get; set; }
        public required string CreatedAt { get; set; }
    }
}
