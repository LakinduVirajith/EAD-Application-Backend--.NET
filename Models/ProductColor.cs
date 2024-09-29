using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class ProductColor
    {
        [Key]
        public int ColorId { get; set; }

        [Required(ErrorMessage = "Color is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Color must be between 2 and 20 characters.")]
        public string Color { get; set; }  = string.Empty;

        // FOREIGN KEY TO PRODUCT
        public string ProductId { get; set; } = string.Empty;
        public ProductModel Product { get; set; }
    }
}
