using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class ProductModel
    {
        [Key]
        public string ProductId { get; set; } = Guid.NewGuid().ToString();

        [StringLength(40, MinimumLength = 5, ErrorMessage = "Image Uri must be between 5 and 40 characters.")]
        public string? ImageUri { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Product Name must be between 2 and 20 characters.")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Brand Name is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Brand Name must be between 2 and 20 characters.")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public Double Price { get; set; } = 0.0;

        [Required(ErrorMessage = "Discount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Discount must be a positive value.")]
        public Double Discount { get; set; } = 0.0;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Category must be between 2 and 20 characters.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stock Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity must be a non-negative integer.")]
        public int StockQuantity { get; set; } = 0;

        // INDICATES WHETHER THE PRODUCT IS VISIBLE TO CUSTOMERS (FOR SOFT DELETE)
        public bool IsVisible { get; set; } = true;

        // NAVIGATION PROPERTIES FOR COLOR AND SIZE
        public virtual ICollection<ProductSize> Sizes { get; set; } = new List<ProductSize>();
        public virtual ICollection<ProductColor> Colors { get; set; } = new List<ProductColor>();
        public virtual ICollection<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();

        // FOREIGN KEY TO VENDOR
        [Required]
        public string VendorId { get; set; } = string.Empty;
        public virtual UserModel User { get; set; } = new UserModel();

        public ProductModel()
        {
            
        }
    }
}
