using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class CartItemModel
    {
        [Key]
        public string CartId { get; set; } = Guid.NewGuid().ToString();

        public string? ImageUri { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Product Name must be between 2 and 20 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public Double Price { get; set; } = 0.0;

        [Required(ErrorMessage = "Discount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Discount must be a positive value.")]
        public Double Discount { get; set; } = 0.0;

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative integer.")]
        public int Quantity { get; set; } = 0;

        // FOREIGN KEY TO PRODUCT
        [Required]
        public string ProductId { get; set; } = string.Empty;
        public virtual ProductModel Product { get; set; }

        // FOREIGN KEY TO CUSTOMER
        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual UserModel User { get; set; }
    }
}
