using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class OrderItemModel
    {
        [Key]
        public string OrderItemId { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Product ID is required.")]
        public required string ProductId { get; set; }

        [Required(ErrorMessage = "Product ID is required.")]
        public required string ProductName { get; set; }

        public string? ImageUri { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public required Double Price { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public required int Quantity { get; set; }

        [Required(ErrorMessage = "Size is required.")]
        public required string Size { get; set; }

        [Required(ErrorMessage = "Color is required.")]
        public required string Color { get; set; }

        // FOREIGN KEY TO ORDER
        [Required(ErrorMessage = "Order ID is required.")]
        public required string OrderId { get; set; }
        public virtual OrderModel Order { get; set; }
    }
}
