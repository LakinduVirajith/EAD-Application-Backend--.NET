using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class ProductColor
    {
        [Key]
        public string ColorId { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Color is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Color must be between 2 and 20 characters.")]
        public required string Color { get; set; }

        // FOREIGN KEY TO PRODUCT
        [Required(ErrorMessage = "Product Id is required.")]
        public required string ProductId { get; set; }
        public ProductModel Product { get; set; }
    }
}
