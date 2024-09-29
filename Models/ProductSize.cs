using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class ProductSize
    {
        [Key]
        public string SizeId { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Size is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Size must be between 2 and 20 characters.")]
        public required string Size { get; set; }

        // FOREIGN KEY TO PRODUCT
        [Required(ErrorMessage = "Product Id is required.")]
        public required string ProductId { get; set; }
        public virtual ProductModel Product { get; set; }
    }
}
