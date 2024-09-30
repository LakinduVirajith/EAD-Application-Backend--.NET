using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class OrderItemModel
    {
        [Key]
        public string OrderItemId { get; set; } = Guid.NewGuid().ToString();
        public required string ProductId { get; set; }
        public required string ProductName { get; set; }
        public string? ImageUri { get; set; }
        public required Double Price { get; set; }
        public required int Quantity { get; set; }
        public required string Size { get; set; }
        public required string Color { get; set; }
        public required string Status { get; set; }

        // FOREIGN KEY TO ORDER
        [Required(ErrorMessage = "Order ID is required.")]
        public required string OrderId { get; set; }
        public virtual OrderModel Order { get; set; }
    }
}
