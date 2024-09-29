using EAD_Backend_Application__.NET.Data;
using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Enums;
using EAD_Backend_Application__.NET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace EAD_Backend_Application__.NET.Services
{
    public class OrderService : IOrderService
    {
        // DEPENDENCIES INJECTED THROUGH CONSTRUCTOR
        private readonly UserManager<UserModel> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        // CONSTRUCTOR TO INJECT DEPENDENCIES
        public OrderService(UserManager<UserModel> userManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<IActionResult> AddOrderAsync(string date, List<OrderItemDTO> itemDTOs)
        {
            // GET THE EMAIL FROM THE AUTHENTICATION HEADER
            var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

            // CHECK IF EMAIL IS NULL
            if (string.IsNullOrEmpty(email))
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "User not found. Please ensure you are logged in." });
            }

            // FIND THE USER BY EMAIL
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "User not found. Please ensure you are logged in." });
            }

            // CHECK USER SHIPPING DETAILS
            if (string.IsNullOrEmpty(user.PhoneNumber) ||
                string.IsNullOrEmpty(user.UserName) ||
                string.IsNullOrEmpty(user.Address) ||
                string.IsNullOrEmpty(user.City) ||
                string.IsNullOrEmpty(user.State) ||
                string.IsNullOrEmpty(user.PostalCode))
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Shipping details are incomplete. Please provide all required shipping information." });
            }
            // CHECK IF ITEM LIST IS EMPTY
            if (itemDTOs.Count == 0)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "The item list cannot be empty. Please add at least one item." });
            }

            // INITIALIZE TOTAL PRICE
            double totalPrice = 0;

            // MAP DTO TO ORDER MODEL
            var order = new OrderModel
            {
                OrderDate = date,
                Status = OrderStatus.Pending.ToString(),
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                Address = user.Address,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                TotalOrderPrice = 0,
                CustomerId = user.Id,
            };

            // MAP EACH ITEM DTO TO ORDER ITEM MODEL
            foreach (var itemDTO in itemDTOs)
            {
                var product = await _context.Products.FindAsync(itemDTO.ProductId);
                if (product == null)
                {
                    return new NotFoundObjectResult(new { Status = "Error", Message = $"Product with ID {itemDTO.ProductId} not found. Please verify the product ID." });
                }
                var orderItem = new OrderItemModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Price = product.Price - (product.Price * product.Discount / 100),
                    Quantity = itemDTO.Quantity,
                    Size = itemDTO.Size,
                    Color = itemDTO.Color,
                    Status = OrderStatus.Pending.ToString(),
                    OrderId = order.OrderId,
                };

                // CALCULATE TOTAL PRICE FOR THIS ORDER ITEM
                totalPrice += orderItem.Price * orderItem.Quantity;

                // ADD THE ORDER ITEM TO THE ORDER'S ITEM COLLECTION
                order.OrderItems.Add(orderItem);
            }

            // UPDATE THE TOTAL ORDER PRICE
            order.TotalOrderPrice = totalPrice;

            try
            {
                // ADD ORDER TO CONTEXT
                _context.Orders.Add(order);

                // SAVE CHANGES
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Status = "Error", Message = "An unexpected error occurred while processing order. Please try again later.", Body = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            return new OkObjectResult(new { Status = "Success", Message = "Order created successfully." });
        }

        public async Task<IActionResult> OrderStatusAsync(string orderId, string status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return new NotFoundObjectResult(new { Status = "Error", Message = "No orders were found for the provided order Id" });
                }

                // Attempt to parse the status to an OrderStatus enum
                if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
                {
                    return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid order status provided." });
                }

                order.Status = status;
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { Status = "Success", Message = "Order status updated successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Status = "Error", Message = "An unexpected error occurred while updating the order status. Please try again later.", Body = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<IActionResult> OrderItemStatusAsync(string orderItemId, string status)
        {
            try
            {
                // FIND THE ORDER ITEM BY ID
                var orderItem = await _context.OrderItems.FindAsync(orderItemId);
                if (orderItem == null)
                {
                    return new NotFoundObjectResult(new { Status = "Error", Message = "No order items were found for the provided order item Id." });
                }

                // ATTEMPT TO PARSE THE STATUS TO AN ORDERSSTATUS ENUM
                if (!Enum.TryParse<OrderStatus>(status, true, out var orderItemStatus))
                {
                    return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid order item status provided." });
                }

                // UPDATE ORDER ITEM STATUS
                orderItem.Status = status;

                // SAVE CHANGES TO THE DATABASE
                await _context.SaveChangesAsync();

                // CHECK IF THE ORDER SHOULD BE MARKED AS DELIVERED
                var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderItem.OrderId);
                if (order != null && order.OrderItems.All(oi => oi.Status == OrderStatus.Delivered.ToString()))
                {
                    // IF ALL ORDER ITEMS ARE DELIVERED, UPDATE THE ORDER STATUS
                    order.Status = OrderStatus.Delivered.ToString();
                    await _context.SaveChangesAsync();
                }

                return new OkObjectResult(new { Status = "Success", Message = "Order item status updated successfully." });
            }
            catch (Exception ex)
            {
                // HANDLE UNEXPECTED ERRORS
                return new ObjectResult(new { Status = "Error", Message = "An unexpected error occurred while updating the order item status. Please try again later.", Body = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<IActionResult> OrderCancellationAsync(OrderCancellationDTO dto)
        {
            try
            {
                var order = await _context.Orders.FindAsync(dto.OrderId);
                if (order == null)
                {
                    return new NotFoundObjectResult(new { Status = "Error", Message = "No orders were found for the provided order Id." });
                }

                order.CancellationReason = dto.CancellationReason;
                order.Status = OrderStatus.Cancelled.ToString();
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { Status = "Success", Message = "Order cancelled successfully." });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Status = "Error", Message = "An unexpected error occurred while cancelling the order. Please try again later.", Body = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ActionResult<IEnumerable<OrderDTO>>> OrderCustomerGetAsync(int pageNumber, int pageSize)
        {
            var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "User not found. Please ensure you are logged in." });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "User not found. Please ensure you are logged in." });
            }

            try
            {
                var orders = await _context.Orders
                    .Where(o => o.CustomerId == user.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (!orders.Any())
                {
                    return new NotFoundObjectResult(new { Status = "Error", Message = "No orders found for this user." });
                }

                var orderDTOs = orders.Select(order => new OrderDTO
                {
                    OrderId = order.OrderId,
                    ImageUri = order.OrderItems.FirstOrDefault()?.ImageUri,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalOrderPrice = order.TotalOrderPrice,
                }).ToList();

                return new OkObjectResult(orderDTOs);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Status = "Error", Message = "An unexpected error occurred while fetching orders. Please try again later.", Body = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ActionResult<IEnumerable<OrderDTO>>> OrderVendorGetAsync(int pageNumber, int pageSize)
        {
            // GET THE EMAIL FROM THE AUTHENTICATION HEADER
            var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

            // CHECK IF EMAIL IS NULL
            if (string.IsNullOrEmpty(email))
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Vendor not found. Please ensure you are logged in." });
            }

            // FIND THE USER BY EMAIL
            var vendor = await _userManager.FindByEmailAsync(email);
            if (vendor == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Vendor not found. Please ensure you are logged in." });
            }

            try
            {
                var products = vendor.Products.ToList();

                if (!products.Any())
                {
                    return new NotFoundObjectResult(new { Status = "Error", Message = "No products found for this vendor." });
                }

                // Fetch order items that match the product IDs
                var orderItems = await _context.OrderItems
                    .Where(oi => products.Select(p => p.ProductId).Contains(oi.ProductId))
                    .ToListAsync();

                if (!orderItems.Any())
                {
                    return new NotFoundObjectResult(new { Status = "Error", Message = "No order items found for the products of this vendor." });
                }

                // Fetch orders associated with the order items
                var orders = await _context.Orders
                    .Where(o => orderItems.Select(oi => oi.OrderId).Contains(o.OrderId))
                    .ToListAsync();

                if (!orders.Any())
                {
                    return new NotFoundObjectResult(new { Status = "Error", Message = "No orders found for the vendor's products." });
                }

                // MAPPING OrderDTO
                var orderDTOs = orders.Select(order => new OrderDTO
                {
                    OrderId = order.OrderId,
                    ImageUri = order.OrderItems.FirstOrDefault()?.ImageUri,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalOrderPrice = order.TotalOrderPrice,
                }).ToList();

                return new OkObjectResult(orderDTOs);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Status = "Error", Message = "An unexpected error occurred while fetching orders. Please try again later.", Body = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ActionResult<IEnumerable<OrderDTO>>> OrderAdminGetAsync(string userEmail, int pageNumber, int pageSize)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.User.Email == userEmail)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (orders == null || !orders.Any())
                {
                    return new NotFoundObjectResult(new { Status = "Error", Message = "No orders found for the specified user." });
                }

                var orderDTOs = orders.Select(order => new OrderDTO
                {
                    OrderId = order.OrderId,
                    ImageUri = order.OrderItems.FirstOrDefault()?.ImageUri,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalOrderPrice = order.TotalOrderPrice,
                }).ToList();

                return new OkObjectResult(orderDTOs);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Status = "Error", Message = "An unexpected error occurred while fetching orders. Please try again later.", Body = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ActionResult<OrderDetailsDTO>> OrderDetailsGetAsync(string orderId)
        {
            try
            {
                // Fetch the order by ID
                var order = await _context.Orders
                    .Include(o => o.OrderItems) // Include order items
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                // Check if the order exists
                if (order == null)
                {
                    return new NotFoundObjectResult(new { Status = "Error", Message = "No orders were found for the provided order Id" });
                }

                // Map the order to OrderDetailsDTO
                var orderDetails = new OrderDetailsDTO
                {
                    OrderId = order.OrderId,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    TotalOrderPrice = order.TotalOrderPrice,
                    PhoneNumber = order.PhoneNumber,
                    UserName = order.UserName,
                    Address = order.Address,
                    City = order.City,
                    State = order.State,
                    PostalCode = order.PostalCode,
                    orderItemDetails = order.OrderItems.Select(oi => new OrderItemDetailsDTO
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ImageResId = oi.ImageUri,
                        Price = oi.Price,
                        Quantity = oi.Quantity.ToString(),
                        Size = oi.Size,
                        Color = oi.Color,
                        Status = OrderStatus.Pending.ToString(),
                    }).ToList()
                };

                return new OkObjectResult(orderDetails);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Status = "Error", Message = "An unexpected error occurred while fetching order details. Please try again later.", Body = ex.Message })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
