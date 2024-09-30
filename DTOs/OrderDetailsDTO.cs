using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class OrderDetailsDTO
    {
        // ORDER DETAILS
        public required string OrderId { get; set; }
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

        // ORDER ITEM DETAILS
        public required List<OrderItemDetailsDTO> orderItemDetails { get; set; }
    }
}
