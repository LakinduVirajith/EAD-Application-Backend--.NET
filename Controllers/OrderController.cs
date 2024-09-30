using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Controllers
{
    [Route("api/v1/order")]
    [ApiController]
    public class OrderController
    {

        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>Adds a list of products to a customer's order.</summary>
        // POST: api/v1/order/add/{date}
        [HttpPost("add/{date}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddOrder(string date)
        {
            return await _orderService.AddOrderAsync(date);
        }

        /// <summary> Allows to change the status of an order.</summary>
        // PUT: api/v1/order/status
        [HttpPut("status")]
        [Authorize(Roles = "Admin, CSR")]
        public async Task<IActionResult> OrderStatus(string orderId, string status)
        {
            return await _orderService.OrderStatusAsync(orderId, status);
        }

        /// <summary> Allows vendors to change the status of an order items.</summary>
        // PUT: api/v1/order/item/status
        [HttpPut("item/status")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> OrderItemStatus(string orderItemId, string status)
        {
            return await _orderService.OrderItemStatusAsync(orderItemId, status);
        }

        /// <summary>Enables authorized users to cancel an order.</summary>
        // PUT: api/v1/order/cancellation
        [HttpPut("cancellation")]
        [Authorize]
        public async Task<IActionResult> OrderCancellation(OrderCancellationDTO dto)
        {
            return await _orderService.OrderCancellationAsync(dto);
        }

        /// <summary>Fetches a paginated list of orders placed by a customer.</summary>
        // GET: api/v1/order/customer/get
        [HttpGet("customer/get")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> OrderCustomerGet(int pageNumber, int pageSize)
        {
            return await _orderService.OrderCustomerGetAsync(pageNumber, pageSize);
        }

        /// <summary>Fetches a paginated list of orders for a vendor.</summary>
        // GET: api/v1/order/vednor/get
        [HttpGet("vendor/get")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult<IEnumerable<OrderItemDetailsDTO>>> OrderVendorGet(int pageNumber, int pageSize)
        {
            return await _orderService.OrderVendorGetAsync(pageNumber, pageSize);
        }

        /// <summary>Allows admin and CSR to view a paginated list of all orders.</summary>
        // GET: api/v1/order/admin/get
        [HttpGet("admin/get")]
        [Authorize(Roles = "Admin, CSR")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> OrderAdminGet(string userEmail, int pageNumber, int pageSize)
        {
            return await _orderService.OrderAdminGetAsync(userEmail, pageNumber, pageSize);
        }

        /// <summary>Retrieves the details of a specific order.</summary>
        // GET: api/v1/order/get/details
        [HttpGet("get/details")]
        [Authorize]
        public async Task<ActionResult<OrderDetailsDTO>> OrderDetailsGet(string orderId)
        {
            return await _orderService.OrderDetailsGetAsync(orderId);
        }
    }
}
