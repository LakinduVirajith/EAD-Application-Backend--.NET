using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Controllers
{
    [Route("api/v1/product")]
    [ApiController]
    public class ProductController
    {

        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // POST: api/v1/product/create
        [HttpPost("create")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> CreateProduct(ProductDetailsDTO dto)
        {
            return await _productService.CreateProductAsync(dto);
        }

        // PUT: api/v1/product/update
        [HttpPut("update")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateProduct(ProductDetailsDTO dto)
        {
            return await _productService.UpdateProductAsync(dto);
        }

        // GET: api/v1/product/all
        [HttpGet("all")]
        [Authorize(Roles = "Vendor, Customer")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts(int pageNumber, int pageSize)
        {
            return await _productService.GetAllProductsAsync(pageNumber, pageSize);
        }

        // GET: api/v1/product/search/{searchValue}
        [HttpGet("search/{searchValue}")]
        [Authorize(Roles = "Vendor, Customer")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> SearchProducts(string searchValue, int pageNumber, int pageSize)
        {
            return await _productService.SearchProductsAsync(searchValue, pageNumber, pageSize);
        }

        // GET: api/v1/product/{productID}
        [HttpGet("{productID}")]
        [Authorize(Roles = "Vendor, Customer")]
        public async Task<ActionResult<ProductDetailsDTO>> GetProduct(string productID)
        {
            return await _productService.GetProductAsync(productID);
        }

        // DELETE: api/v1/product/{productID}
        [HttpDelete("{productID}")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> DeleteProduct(string productID)
        {
            return await _productService.DeleteProductAsync();
        }
    }
}
