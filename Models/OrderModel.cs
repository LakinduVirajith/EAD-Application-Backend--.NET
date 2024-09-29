using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.Models
{
    public class OrderModel
    {
        // ORDER DETAILS
        [Key]
        public string OrderId { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Order Date is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Order Date must be in 'yyyy-MM-dd' format.")]
        public required string OrderDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Statust be between 2 and 10 haracters.")]
        public required string Status { get; set; }

        [Required(ErrorMessage = "Total Order Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public required Double TotalOrderPrice { get; set; }

        // SHIPPING DETAILS
        [Required(ErrorMessage = "Phone Number is required.")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public required string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public required string State { get; set; }

        [Required(ErrorMessage = "Postal Code is required.")]
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
