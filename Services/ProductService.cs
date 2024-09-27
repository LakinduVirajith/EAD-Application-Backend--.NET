using EAD_Backend_Application__.NET.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Services
{
    public class ProductService : IProductService
    {
        public Task<IActionResult> CreateProductAsync(ProductDetailsDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> DeleteProductAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<ProductDetailsDTO>> GetProductAsync(string productID)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult<IEnumerable<ProductDTO>>> SearchProductsAsync(string searchValue, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> UpdateProductAsync(ProductDetailsDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
