using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class ProductSize
    {
        [Key]
        public int SizeId { get; set; }

        [Required(ErrorMessage = "Size is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Size must be between 2 and 20 characters.")]
        public string Size { get; set; } = string.Empty;

        // FOREIGN KEY TO PRODUCT
        public string ProductId { get; set; } = string.Empty;
        public virtual ProductModel Product { get; set; }
    }
}
