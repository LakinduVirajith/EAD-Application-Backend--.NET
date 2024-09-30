using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class RankingDTO
    {
        [Required(ErrorMessage = "Vendor Id is required.")]
        public required string VendorId { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Comment must be between 2 and 200 characters.")]
        public required string Comment { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Rating must be a non-negative integer.")]
        public required int Rating { get; set; } = 0;

        [Required(ErrorMessage = "Created Date is required.")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Created Date must be in 'yyyy-MM-dd' format.")]
        public required string CreatedAt { get; set; }
    }
}
