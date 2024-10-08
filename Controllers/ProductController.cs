using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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

        /// <summary>Allows a vendor to create a new product.</summary>
        // POST: api/v1/product/create
        [HttpPost("create")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> CreateProduct(ProductCreateDTO dto)
        {
            return await _productService.CreateProductAsync(dto);
        }

        /// <summary>Allows a vendor to update product image of an existing product.</summary>
        // PUT: api/v1/product/image
        [HttpPut("image")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateProductImage([FromForm] FileUploadDTO dto)
        {
            if (dto == null || dto.File.Length == 0)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Please provide a valid image file." });
            }
            return await _productService.UpdateProductImageAsync(dto.File);
        }

        /// <summary>Allows a vendor to update details of an existing product.</summary>
        // PUT: api/v1/product/update
        [HttpPut("update")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> UpdateProduct(ProductDetailsDTO dto)
        {
            return await _productService.UpdateProductAsync(dto);
        }

        /// <summary>Allows a vendor to update details of an existing product.</summary>
        // PUT: api/v1/product/update
        [HttpPut("stock")]
        [Authorize(Roles = "Vendor, Admin")]
        public async Task<IActionResult> UpdateProductStock(ProductStockDTO dto)
        {
            return await _productService.UpdateProductStockAsync(dto);
        }

        /// <summary>Retrieves a paginated list of all products available.</summary>
        // GET: api/v1/product/all
        [HttpGet("home")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts(int pageNumber, int pageSize)
        {
            return await _productService.GetAllProductsAsync(pageNumber, pageSize);
        }

        /// <summary>Searches for products based on the given search value.</summary>
        // GET: api/v1/product/search/{searchValue}
        [HttpGet("search/{searchValue}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> SearchProducts(string searchValue, int pageNumber, int pageSize)
        {
            return await _productService.SearchProductsAsync(searchValue, pageNumber, pageSize);
        }

        /// <summary>Searches for products based on the given product category.</summary>
        // GET: api/v1/product/category/{category}
        [HttpGet("category/{category}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> SearchProductCategory(string category, int pageNumber, int pageSize)
        {
            return await _productService.SearchProductCategoryAsync(category, pageNumber, pageSize);
        }

        /// <summary>Retrieves a paginated list of products created by the vendor.</summary>
        // GET: api/v1/product/vendor
        [HttpGet("vendor")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByVendor(int pageNumber, int pageSize)
        {
            return await _productService.GetProductsByVendorAsync(pageNumber, pageSize);
        }

        /// <summary>Retrieves details for a specific product by its ID.</summary>
        // GET: api/v1/product/{productID}
        [HttpGet("{productID}")]
        [Authorize(Roles = "Vendor, Customer")]
        public async Task<ActionResult<ProductDetailsDTO>> GetProduct(string productID)
        {
            return await _productService.GetProductAsync(productID);
        }

        /// <summary>Allows a vendor to delete a product by its ID.</summary>
        // DELETE: api/v1/product/{productID}
        [HttpDelete("{productID}")]
        [Authorize(Roles = "Vendor, Admin")]
        public async Task<IActionResult> DeleteProduct(string productID)
        {
            return await _productService.DeleteProductAsync(productID);
        }

        /// <summary>Retrieves a paginated list of active products.</summary>
        // GET: api/v1/product/active
        [HttpGet("active")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetActiveProduct(int pageNumber, int pageSize)
        {
            return await _productService.GetActiveProductAsync(pageNumber, pageSize);
        }

        /// <summary>Retrieves a paginated list of inactive products.</summary>
        // GET: api/v1/product/inactive
        [HttpGet("inactive")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetInactiveProduct(int pageNumber, int pageSize)
        {
            return await _productService.GetInactiveProductAsync(pageNumber, pageSize);
        }

        /// <summary>Updates the status of a specific product.</summary>
        // PUT: api/v1/product/status
        [HttpPut("status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductStatus(string productID, bool status)
        {
            return await _productService.UpdateProductStatusAsync(productID, status);
        }
    }
}
