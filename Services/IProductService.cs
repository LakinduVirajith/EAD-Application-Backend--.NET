using EAD_Backend_Application__.NET.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Services
{
    public interface IProductService
    {
        Task<IActionResult> CreateProductAsync(ProductCreateDTO dto);
        Task<IActionResult> UpdateProductImageAsync(IFormFile imageFile);
        Task<IActionResult> UpdateProductAsync(ProductDetailsDTO dto);
        Task<IActionResult> UpdateProductStockAsync(ProductStockDTO dto);
        Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProductsAsync(int pageNumber, int pageSize);
        Task<ActionResult<IEnumerable<ProductDTO>>> SearchProductsAsync(string searchValue, int pageNumber, int pageSize);
        Task<ActionResult<IEnumerable<ProductDTO>>> SearchProductCategoryAsync(string category, int pageNumber, int pageSize);
        Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByVendorAsync(int pageNumber, int pageSize);
        Task<ActionResult<ProductDetailsDTO>> GetProductAsync(string productID);
        Task<IActionResult> DeleteProductAsync(string productID);
        Task<ActionResult<IEnumerable<ProductDTO>>> GetActiveProductAsync(int pageNumber, int pageSize);
        Task<ActionResult<IEnumerable<ProductDTO>>> GetInactiveProductAsync(int pageNumber, int pageSize);
        Task<IActionResult> UpdateProductStatusAsync(string productID, bool status);
    }
}
