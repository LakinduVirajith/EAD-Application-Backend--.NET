namespace EAD_Backend_Application__.NET.DTOs
{
    public class UserShippingDetailsDTO
    {
        public required string Address { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string PostalCode { get; set; }
    }
}
