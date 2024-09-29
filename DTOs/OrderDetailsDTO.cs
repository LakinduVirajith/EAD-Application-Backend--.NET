using System.ComponentModel.DataAnnotations;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class OrderDetailsDTO
    {
        // ORDER DETAILS
        public string OrderId { get; set; } = string.Empty;
        public string OrderDate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Double TotalOrderPrice { get; set; }

        // SHIPPING DETAILS
        public string PhoneNumber { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        // ORDER ITEM DETAILS
        public List<OrderItemDetailsDTO> orderItemDetails { get; set; } = new List<OrderItemDetailsDTO>();
        
        public OrderDetailsDTO()
        {
            
        }
    }
}
