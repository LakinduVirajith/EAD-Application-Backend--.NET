using EAD_Backend_Application__.NET.Data;
using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace EAD_Backend_Application__.NET.Services
{
    public class ProductService : IProductService
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public ProductService(UserManager<UserModel> userManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<IActionResult> CreateProductAsync(ProductDetailsDTO dto)
        {
            // GET THE EMAIL FROM THE AUTHENTICATION HEADER
            var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;


            // CHECK IF EMAIL IS NULL
            if (string.IsNullOrEmpty(email))
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Vendor not found. Please ensure you are logged in." });
            }
            // FIND THE VENDOR BY EMAIL AND IF VENDOR IS NULL
            var vendor = await _userManager.FindByEmailAsync(email);
            if (vendor == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Vendor not found. Please ensure you are logged in." });
            }

            // MAP DTO TO PRODUCTMODEL
            var product = new ProductModel
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Price = dto.Price,
                Discount = dto.Discount,
                Description = dto.Description,
                Category = dto.Category,
                StockQuantity = dto.StockQuantity,
                IsVisible = dto.IsVisible,
                VendorId = vendor.Id
            };

            // MAP SIZES AND COLORS FROM DTO TO PRODUCTMODEL
            product.Sizes = dto.Size.Select(size => new ProductSize { Size = size }).ToList();
            product.Colors = dto.Color.Select(color => new ProductColor { Color = color }).ToList();

            // SAVE TO DATABASE (USING DBCONTEXT)
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { Status = "Success", Message = "Product successfully saved to the database.", body = product.ProductId });
            }
            catch (Exception)
            {
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while saving the product. Please try again later." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<IActionResult> UpdateProductImageAsync(IFormFile imageFile)
        {
            return await Task.FromResult(new NotFoundObjectResult(new { Status = "Error", Message = "This feature is currently under development and is not yet implemented." }));
        }

        public async Task<IActionResult> UpdateProductAsync(ProductDetailsDTO dto)
        {
            // FIND THE PRODUCT BY ID
            var product = await _context.Products.FindAsync(dto.ProductId);

            // CHECK IF PRODUCT EXISTS
            if (product == null)
            {
                return new NotFoundObjectResult(new { Statsu = "Error", Message = "Product not found. Please check the provided ID." });
            }

            // UPDATE PRODUCT PROPERTIES
            product.ImageUri = dto.ImageUri;
            product.Name = dto.Name;
            product.Brand = dto.Brand;
            product.Price = dto.Price;
            product.Discount = dto.Discount;
            product.Description = dto.Description;
            product.Category = dto.Category;
            product.StockQuantity = dto.StockQuantity;
            product.IsVisible = dto.IsVisible;

            // UPDATE SIZES AND COLORS
            product.Sizes = dto.Size.Select(size => new ProductSize { Size = size }).ToList();
            product.Colors = dto.Color.Select(color => new ProductColor { Color = color }).ToList();

            // SAVE CHANGES TO DATABASE
            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { Status = "Success", Message = "Product successfully updated in the database." });
            }
            catch (Exception)
            {
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while updating the product. Please try again later." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            // VALIDATE PAGE NUMBER AND SIZE
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid page number or page size." });
            }

            try
            {
                // FETCH TOTAL PRODUCT COUNT
                var totalProducts = await _context.Products.CountAsync();

                // FETCH PAGED PRODUCTS
                var products = await _context.Products
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Price = p.Price,
                        Discount = p.Discount
                    })
                    .ToListAsync();

                // CREATE PAGINATION RESPONSE
                var paginationResponse = new
                {
                    Status = "Success",
                    TotalCount = totalProducts,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Products = products
                };

                return new OkObjectResult(new { Status = "Success", Body = paginationResponse });
            }
            catch (Exception)
            {
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while retrieving the products. Please try again latert." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ActionResult<IEnumerable<ProductDTO>>> SearchProductsAsync(string searchValue, int pageNumber, int pageSize)
        {
            // VALIDATE PAGE NUMBER AND SIZE
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid page number or page size." });
            }

            try
            {
                // FETCH TOTAL PRODUCT COUNT BASED ON SEARCH VALUE
                var totalProducts = await _context.Products
                    .Where(p => p.Name.Contains(searchValue) || p.Description.Contains(searchValue))
                    .CountAsync();

                // FETCH PAGED PRODUCTS BASED ON SEARCH VALUE
                var products = await _context.Products
                    .Where(p => p.Name.Contains(searchValue) || p.Description.Contains(searchValue))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Price = p.Price,
                        Discount = p.Discount
                    })
                    .ToListAsync();

                // CREATE PAGINATION RESPONSE
                var paginationResponse = new
                {
                    Status = "Success",
                    TotalCount = totalProducts,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Products = products
                };

                return new OkObjectResult(new { Status = "Success", Body = paginationResponse });
            }
            catch (Exception)
            {
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while searching for products. Please try again later." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductsByVendorAsync(int pageNumber, int pageSize)
        {
            // GET THE EMAIL FROM THE AUTHENTICATION HEADER
            var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;


            // CHECK IF EMAIL IS NULL
            if (string.IsNullOrEmpty(email))
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Vendor not found. Please ensure you are logged in." });
            }
            // FIND THE VENDOR BY EMAIL AND IF VENDOR IS NULL
            var vendor = await _userManager.FindByEmailAsync(email);
            if (vendor == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Vendor not found. Please ensure you are logged in." });
            }
            // VALIDATE PAGE NUMBER AND SIZE
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid page number or page size." });
            }

            try
            {
                // FETCH TOTAL PRODUCT COUNT BASED ON VENDOR ID
                var totalProducts = await _context.Products
                .Where(p => p.VendorId.Equals(vendor.Id))
                    .CountAsync();

                // FETCH PAGED PRODUCTS BASED ON VENDOR ID
                var products = await _context.Products
                .Where(p => p.VendorId.Equals(vendor.Id))
                .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Price = p.Price,
                        Discount = p.Discount
                    })
                    .ToListAsync();

                // CREATE PAGINATION RESPONSE
                var paginationResponse = new
                {
                    Status = "Success",
                    TotalCount = totalProducts,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Products = products
                };

                return new OkObjectResult(new { Status = "Success", Body = paginationResponse });
            }
            catch (Exception)
            {
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while retrieving the products. Please try again latert." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ActionResult<ProductDetailsDTO>> GetProductAsync(string productID)
        {
            try
            {
                // FETCH THE PRODUCT BY ID
                var product = await _context.Products
                    .Include(p => p.Sizes) // Include sizes if needed
                    .Include(p => p.Colors) // Include colors if needed
                    .FirstOrDefaultAsync(p => p.ProductId.ToString() == productID);

                // CHECK IF PRODUCT EXISTS
                if (product == null)
                {
                    return new NotFoundObjectResult(new { Statsu = "Error", Message = "Product not found. Please check the provided ID." });
                }

                // MAP TO PRODUCT DETAILS DTO
                var productDetails = new ProductDetailsDTO
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Brand = product.Brand,
                    Price = product.Price,
                    Discount = product.Discount,
                    Description = product.Description,
                    Category = product.Category,
                    StockQuantity = product.StockQuantity,
                    IsVisible = product.IsVisible,
                    Size = product.Sizes.Select(s => s.Size).ToList(),
                    Color = product.Colors.Select(c => c.Color).ToList()
                };

                return new OkObjectResult(new { Status = "Success", Body = productDetails });
            }
            catch (Exception)
            {
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while fetching the product. Please try again later." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
        
        public async Task<IActionResult> DeleteProductAsync(string productID)
        {
            try
            {
                // FIND THE PRODUCT BY ID
                var product = await _context.Products.FindAsync(productID);

                // CHECK IF PRODUCT EXISTS
                if (product == null)
                {
                    return new NotFoundObjectResult(new { Statsu = "Error", Message = "Product not found. Please check the provided ID." });
                }

                // REMOVE THE PRODUCT FROM DATABASE
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { Status = "Success", Message = "Product successfully deleted from the database." });
            }
            catch (Exception)
            {
                return new ConflictObjectResult(new { Status = "Error", Message = "Product cannot be deleted. It is currently in use. Please contact administration." });
            }
        }
    }
}
