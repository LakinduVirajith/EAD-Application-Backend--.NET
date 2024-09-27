using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class ProductColor
    {
        [Key]
        public int ColorId { get; set; }

        [Required]
        [StringLength(20)]
        public string Color { get; set; }  = string.Empty;

        // FOREIGN KEY TO PRODUCT
        public string ProductId { get; set; } = string.Empty;
        public virtual ProductModel Product { get; set; }

        public ProductColor()
        {
            
        }
    }
}
