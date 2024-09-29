using EAD_Backend_Application__.NET.Data;
using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace EAD_Backend_Application__.NET.Services
{
    public class CartService : ICartService
    {
        // DEPENDENCIES INJECTED THROUGH CONSTRUCTOR
        private readonly UserManager<UserModel> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        // CONSTRUCTOR TO INJECT DEPENDENCIES
        public CartService(UserManager<UserModel> userManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<IActionResult> AddProductAsync(CartAddDTO dto)
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

            // FIND THE PRODUCT IN DATABASE
            var product = await _context.Products.FindAsync(dto.productId);
            if (product == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Product not found." });
            }

            // VALIDATE THE QUANTITY 
            if (dto.Quantity > product.StockQuantity)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Not a valid stock quantity." });
            }

            // CHECK IF CART ITEM ALREADY EXISTS
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == user.Id && ci.ProductId == dto.productId);
            if (cartItem != null)
            {
                // INCREASE QUANTITY IF PRODUCT ALREADY IN CART
                cartItem.Quantity++;
            }
            else
            {
                // CREATE NEW CART ITEM IF NOT EXISTING
                cartItem = new CartItemModel
                {
                    Quantity = dto.Quantity,
                    Size = dto.productId,
                    Color = dto.Color,
                    ProductId = dto.productId,
                    UserId = user.Id
                };
                _context.CartItems.Add(cartItem);
            }

            // SAVE CHANGES TO DATABASE
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { Status = "Success", Message = "Product added to cart." });
        }

        public async Task<IActionResult> IncreaseQuantityAsync(string cartId)
        {
            // FIND THE CART ITEM
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.CartId == cartId);
            if (cartItem == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Cart item not found." });
            }

            // FIND THE PRODUCT BY PRODUCT ID
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);
            if (product == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Product not found." });
            }

            // CHECK IF THERE IS ENOUGH STOCK
            if (cartItem.Quantity >= product.StockQuantity)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Not enough stock available." });
            }

            // INCREASE QUANTITY
            cartItem.Quantity++;

            // SAVE CHANGES TO DATABASE
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { Status = "Success", Message = "Product quantity increased." });
        }

        public async Task<IActionResult> DecreaseQuantityAsync(string cartId)
        {
            // FIND THE CART ITEM
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.CartId == cartId);
            if (cartItem == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Cart item not found." });
            }

            // DECREASE QUANTITY OR REMOVE ITEM IF QUANTITY IS 1
            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
            }
            else
            {
                _context.CartItems.Remove(cartItem);
            }

            // SAVE CHANGES TO DATABASE
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { Status = "Success", Message = "Product quantity decreased." });
        }

        public async Task<ActionResult<IEnumerable<CartItemDTO>>> GetProductsAsync()
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

            // FETCH CART ITEMS FOR THE USER
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == user.Id)
                .ToListAsync();

            // LIST TO HOLD CART ITEMS DTO
            var cartItemDtos = new List<CartItemDTO>();

            foreach (var cartItem in cartItems)
            {
                // FIND THE PRODUCT BY PRODUCT ID
                var product = await _context.Products
                    .Include(p => p.Sizes)
                    .Include(p => p.Colors)
                    .FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);

                if (product == null)
                {
                    // REMOVE CART ITEM
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    // COMPARE CART QUANTITY WITH STOCK QUANTITY
                    if (cartItem.Quantity > product.StockQuantity)
                    {    
                        cartItem.Quantity = product.StockQuantity;
                    }
                    // COMPARE SIZE
                    var productSize = product.Sizes.FirstOrDefault(ps => ps.Size == cartItem.Size);
                    if (productSize == null)
                    {
                        // ASSIGN FIRST AVAILABLE SIZE IF CART SIZE NOT AVAILABLE
                        cartItem.Size = product.Sizes.FirstOrDefault()?.Size ?? "Unknown";
                    }

                    // COMPARE COLOR
                    var productColor = product.Colors.FirstOrDefault(pc => pc.Color == cartItem.Color);
                    if (productColor == null)
                    {
                        // ASSIGN FIRST AVAILABLE COLOR IF CART COLOR NOT AVAILABLE
                        cartItem.Color = product.Colors.FirstOrDefault()?.Color ?? "Unknown";
                    }

                    // SAVE CHANGES TO CART ITEM
                    _context.CartItems.Update(cartItem);

                    // ADD TO CART ITEM DTO
                    cartItemDtos.Add(new CartItemDTO
                    {
                        CartId = cartItem.CartId,
                        ImageUri = product.ImageUri,
                        ProductName = product.Name,
                        Price = product.Price,
                        Discount = product.Discount,
                        Size = cartItem.Size,
                        Color = cartItem.Color,
                        Quantity = cartItem.Quantity,
                        ProductId = cartItem.ProductId
                    });
                }
            }

            // SAVE ANY CHANGES TO THE DATABASE
            await _context.SaveChangesAsync();

            return new OkObjectResult(cartItemDtos);
        }

        public async Task<IActionResult> RemoveProductAsync(string cartId)
        {
            // FIND THE CART ITEM
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.CartId == cartId);
            if (cartItem == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Cart item not found." });
            }

            // REMOVE CART ITEM
            _context.CartItems.Remove(cartItem);

            // SAVE CHANGES TO DATABASE
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { Status = "Success", Message = "Product removed from cart." });
        }
    }
}
