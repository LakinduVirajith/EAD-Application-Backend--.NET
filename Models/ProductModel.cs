using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class ProductModel
    {
        [Key]
        public string ProductId { get; set; } = Guid.NewGuid().ToString().Substring(0, 16);
        public string? ImageUri { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public Double Price { get; set; } = 0.0;
        public Double Discount { get; set; } = 0.0;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int StockQuantity { get; set; } = 0;

        // INDICATES WHETHER THE PRODUCT IS VISIBLE TO CUSTOMERS (FOR SOFT DELETE)
        public bool IsVisible { get; set; } = false;

        // NAVIGATION PROPERTIES FOR COLOR AND SIZE
        public virtual ICollection<ProductSize> Sizes { get; set; } = new List<ProductSize>();
        public virtual ICollection<ProductColor> Colors { get; set; } = new List<ProductColor>();
        public virtual ICollection<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();

        // FOREIGN KEY TO VENDOR
        [Required]
        public string VendorId { get; set; } = string.Empty;
        public virtual UserModel Vendor { get; set; }
    }
}
