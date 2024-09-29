using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class CartItemModel
    {
        [Key]
        public string CartId { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Size is required.")]
        public string Size { get; set; } = string.Empty;

        [Required(ErrorMessage = "Color is required.")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required.")]
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
