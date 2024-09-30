using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class OrderModel
    {
        // ORDER DETAILS
        [Key]
        public string OrderId { get; set; } = Guid.NewGuid().ToString();
        public required string OrderDate { get; set; }
        public required string Status { get; set; }
        public required Double TotalOrderPrice { get; set; }

        // SHIPPING DETAILS
        public required string PhoneNumber { get; set; }
        public required string UserName { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }

        // OPTIONAL: CANCELLATION REASON
        public string? CancellationReason { get; set; }

        // NAVIGATION PROPERTIES FOR ORDER ITEMS
        public virtual ICollection<OrderItemModel> OrderItems { get; set; }

        // FOREIGN KEY TO CUSTOMER
        [Required]
        public required string CustomerId { get; set; }
        public virtual UserModel User { get; set; }
    }
}
