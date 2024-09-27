using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class ProductModel
    {
        [Key]
        public string ProductId { get; set; } = Guid.NewGuid().ToString();
        
        [Url]
        public string? ImageUri { get; set; }
        
        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Brand { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue)]
        public Double Price { get; set; } = 0.0;
        
        [Range(0, double.MaxValue)]
        public Double Discount { get; set; } = 0.0;

        // NAVIGATION PROPERTIES FOR COLOR AND SIZE
        public virtual ICollection<ProductSize> Sizes { get; set; } = new List<ProductSize>();
        public virtual ICollection<ProductColor> Colors { get; set; } = new List<ProductColor>();

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Category { get; set; } = string.Empty;
        
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        // INDICATES WHETHER THE PRODUCT IS VISIBLE TO CUSTOMERS (FOR SOFT DELETE)
        public bool IsVisible { get; set; } = true;

        // IDENTIFIER FOR THE VENDOR ASSOCIATED WITH THE PRODUCT
        [Required]
        public string VendorId { get; set; } = string.Empty;
        public virtual UserModel User { get; set; }

        public ProductModel()
        {
            
        }
    }
}
