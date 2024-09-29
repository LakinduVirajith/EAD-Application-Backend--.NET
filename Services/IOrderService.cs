using EAD_Backend_Application__.NET.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Services
{
    public interface IOrderService
    {
        Task<IActionResult> AddOrderAsync(string date, List<OrderItemDTO> itemDTOs);
        Task<IActionResult> OrderStatusAsync(string orderId, string status);
        Task<IActionResult> OrderItemStatusAsync(string orderItemId, string status);
        Task<IActionResult> OrderCancellationAsync(OrderCancellationDTO dto);
        Task<ActionResult<IEnumerable<OrderDTO>>> OrderCustomerGetAsync(int pageNumber, int pageSize);
        Task<ActionResult<IEnumerable<OrderDTO>>> OrderVendorGetAsync(int pageNumber, int pageSize);
        Task<ActionResult<IEnumerable<OrderDTO>>> OrderAdminGetAsync(string userEmail, int pageNumber, int pageSize);
        Task<ActionResult<OrderDetailsDTO>> OrderDetailsGetAsync(string orderId);
    }
}
