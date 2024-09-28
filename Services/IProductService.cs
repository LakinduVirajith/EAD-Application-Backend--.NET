using EAD_Backend_Application__.NET.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Services
{
    public interface IProductService
    {
        Task<IActionResult> CreateProductAsync(ProductDetailsDTO dto);
        Task<IActionResult> UpdateProductImageAsync(IFormFile imageFile);
        Task<IActionResult> UpdateProductAsync(ProductDetailsDTO dto);
        Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProductsAsync(int pageNumber, int pageSize);
        Task<ActionResult<IEnumerable<ProductDTO>>> SearchProductsAsync(string searchValue, int pageNumber, int pageSize);
        Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByVendorAsync(int pageNumber, int pageSize);
        Task<ActionResult<ProductDetailsDTO>> GetProductAsync(string productID);
        Task<IActionResult> DeleteProductAsync(string productID);
    }
}
