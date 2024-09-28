using EAD_Backend_Application__.NET.Data;
using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> AddProductAsync(string productId)
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
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Product not found." });
            }

            // CHECK IF CART ITEM ALREADY EXISTS
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == user.Id && ci.ProductId == productId);
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
                    ProductName = product.Name,
                    Price = product.Price,
                    Discount = product.Discount,
                    Quantity = 1,
                    ProductId = productId,
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
            return new OkObjectResult(new { Status = "Success", Message = "Product quantity updated." });
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
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);
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
                    // COMPARE OTHER ENTITIES
                    if(cartItem.ImageUri != product.ImageUri)
                    {
                        cartItem.ImageUri = product.ImageUri;
                    }
                    if (cartItem.Price != product.Price) 
                    { 
                        cartItem.Price = product.Price;
                    }
                    if (cartItem.Discount != product.Discount)
                    {
                        cartItem.Discount = product.Discount;
                    }

                    // SAVE CHANGES TO CART ITEM
                    _context.CartItems.Update(cartItem);

                    // ADD TO CART ITEM DTO
                    cartItemDtos.Add(new CartItemDTO
                    {
                        CartId = cartItem.CartId,
                        ImageUri = cartItem.ImageUri,
                        ProductName = cartItem.ProductName,
                        Price = cartItem.Price,
                        Discount = cartItem.Discount,
                        Quantity = cartItem.Quantity,
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
