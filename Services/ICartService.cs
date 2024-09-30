using EAD_Backend_Application__.NET.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Services
{
    public interface ICartService
    {
        Task<IActionResult> AddProductAsync(CartAddDTO dto);
        Task<IActionResult> IncreaseQuantityAsync(string cartId);
        Task<IActionResult> DecreaseQuantityAsync(string cartId);
        Task<ActionResult<IEnumerable<CartItemDTO>>> GetProductsAsync();
        Task<IActionResult> RemoveProductAsync(string cartId);
    }
}
