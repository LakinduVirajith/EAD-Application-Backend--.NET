using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Controllers
{
    [Route("api/v1/cart")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CartController
    {
        private readonly ICartService _cartService;

         public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>Adds a product to the cart.</summary>
        // POST: api/v1/cart/add
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct(CartAddDTO dto) { 
            return await _cartService.AddProductAsync(dto);
        }

        /// <summary>Increases the quantity of a product in the cart.</summary>
        // PUT: api/v1/cart/plus/{productId}
        [HttpPut("plus/{cartId}")]
        public async Task<IActionResult> IncreaseQuantity(string cartId)
        {
            return await _cartService.IncreaseQuantityAsync(cartId);
        }

        /// <summary>Decreases the quantity of a product in the cart.</summary>
        // PUT: api/v1/cart/minus/{productId}
        [HttpPut("minus/{cartId}")]
        public async Task<IActionResult> DecreaseQuantity(string cartId)
        {
            return await _cartService.DecreaseQuantityAsync(cartId);
        }

        /// <summary>Retrieves all products in the cart.</summary>
        // GET: api/v1/cart/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<CartItemDTO>>> GetProducts()
        {
            return await _cartService.GetProductsAsync();
        }

        /// <summary>Removes a product from the cart.</summary>
        // PUT: api/v1/cart/remove/{productId}
        [HttpDelete("remove/{cartId}")]
        public async Task<IActionResult> RemoveProduct(string cartId)
        {
            return await _cartService.RemoveProductAsync(cartId);
        }
    }
}
