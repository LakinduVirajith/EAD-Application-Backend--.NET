using EAD_Backend_Application__.NET.Data;
using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Enums;
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

        public async Task<IActionResult> CreateProductAsync(ProductCreateDTO dto)
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

            // VALIDATE PRODUCT CATEGORY
            if (!Enum.IsDefined(typeof(ProductCategory), dto.Category))
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid product category." });
            }

            // MAP DTO TO PRODUCT MODEL
            var product = new ProductModel
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Price = dto.Price,
                Discount = dto.Discount,
                Description = dto.Description,
                Category = dto.Category,
                StockQuantity = dto.StockQuantity,
                IsVisible = false,
                VendorId = vendor.Id
            };

            // MAP SIZES AND COLORS FROM DTO TO PRODUCTMODEL
            product.Sizes = dto.Size.Select(size => new ProductSize { Size = size, ProductId = product.ProductId }).ToList();
            product.Colors = dto.Color.Select(color => new ProductColor { Color = color, ProductId = product.ProductId }).ToList();

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
            var product = await _context.Products
            .Include(p => p.Sizes)
            .Include(p => p.Colors)
            .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);

            // CHECK IF PRODUCT EXISTS
            if (product == null)
            {
                return new NotFoundObjectResult(new { Statsu = "Error", Message = "Product not found. Please check the provided ID." });
            }

            // VALIDATE PRODUCT CATEGORY
            if (!Enum.IsDefined(typeof(ProductCategory), dto.Category))
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid product category." });
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

            // CLEAR EXISTING SIZES AND COLORS
            product.Sizes.Clear();
            product.Colors.Clear();

            // UPDATE SIZES AND COLORS
            product.Sizes = dto.Size.Select(size => new ProductSize { Size = size, ProductId = product.ProductId }).ToList();
            product.Colors = dto.Color.Select(color => new ProductColor { Color = color, ProductId = product.ProductId }).ToList();

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

        public async Task<IActionResult> UpdateProductStockAsync(ProductStockDTO dto)
        {
            // FIND THE PRODUCT BY ID
            var product = await _context.Products.FindAsync(dto.ProductId);

            // CHECK IF PRODUCT EXISTS
            if (product == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "Product not found. Please check the provided ID." });
            }

            // UPDATE THE STOCK QUANTITY
            product.StockQuantity = dto.StockQuantity;

            // SAVE CHANGES TO DATABASE
            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { Status = "Success", Message = "Product stock successfully updated." });
            }
            catch (Exception)
            {
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while updating the stock. Please try again later." })
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
                var totalProducts = await _context.Products.CountAsync(p => p.IsVisible);

                // FETCH PAGED PRODUCTS
                var products = await _context.Products
                    .Where(p => p.IsVisible)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Price = p.Price,
                        Discount = p.Discount,
                        StockQuantity = p.StockQuantity
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
                    .Where(p => (p.Name.Contains(searchValue) || p.Description.Contains(searchValue)) && p.IsVisible)
                    .CountAsync();

                // FETCH PAGED PRODUCTS BASED ON SEARCH VALUE
                var products = await _context.Products
                    .Where(p => (p.Name.Contains(searchValue) || p.Description.Contains(searchValue)) && p.IsVisible)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Price = p.Price,
                        Discount = p.Discount,
                        StockQuantity = p.StockQuantity
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

        public async Task<ActionResult<IEnumerable<ProductDTO>>> SearchProductCategoryAsync(string category, int pageNumber, int pageSize)
        {
            // VALIDATE PAGE NUMBER AND SIZE
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid page number or page size." });
            }

            try
            {
                // VALIDATE THE CATEGORY USING THE ENUM
                if (!Enum.TryParse<ProductCategory>(category, true, out var validCategory))
                {
                    return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid product category provided." });
                }

                // FETCH TOTAL PRODUCT COUNT BASED ON CATEGORY
                var totalProducts = await _context.Products
                    .Where(p => p.Category.Equals(category) && p.IsVisible)
                    .CountAsync();

                // FETCH PAGED PRODUCTS BASED ON CATEGORY
                var products = await _context.Products
                    .Where(p => p.Category.Equals(category) && p.IsVisible)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Price = p.Price,
                        Discount = p.Discount,
                        StockQuantity = p.StockQuantity
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
            catch (Exception ex)
            {
                // HANDLE ANY UNEXPECTED ERRORS
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while searching for products by category. Please try again later.", Body = ex.Message })
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
                        Discount = p.Discount,
                        StockQuantity = p.StockQuantity
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
                    .Include(p => p.Sizes)
                    .Include(p => p.Colors)
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
                    ImageUri = product.ImageUri,
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

        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetActiveProductAsync(int pageNumber, int pageSize)
        {
            // VALIDATE PAGE NUMBER AND SIZE
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid page number or page size." });
            }

            try
            {
                // FETCH TOTAL ACTIVE PRODUCT COUNT
                var totalActiveProducts = await _context.Products
                    .Where(p => p.IsVisible)
                    .CountAsync();

                // FETCH PAGED ACTIVE PRODUCTS
                var activeProducts = await _context.Products
                    .Where(p => p.IsVisible)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Price = p.Price,
                        Discount = p.Discount,
                        StockQuantity = p.StockQuantity
                    })
                    .ToListAsync();

                // CREATE PAGINATION RESPONSE
                var paginationResponse = new
                {
                    Status = "Success",
                    TotalCount = totalActiveProducts,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Products = activeProducts
                };

                return new OkObjectResult(new { Status = "Success", Body = paginationResponse });
            }
            catch (Exception)
            {
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while retrieving active products. Please try again later." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetInactiveProductAsync(int pageNumber, int pageSize)
        {
            // VALIDATE PAGE NUMBER AND SIZE
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BadRequestObjectResult(new { Status = "Error", Message = "Invalid page number or page size." });
            }

            try
            {
                // FETCH TOTAL INACTIVE PRODUCT COUNT
                var totalInactiveProducts = await _context.Products
                    .Where(p => !p.IsVisible)
                    .CountAsync();

                // FETCH PAGED INACTIVE PRODUCTS
                var inactiveProducts = await _context.Products
                    .Where(p => !p.IsVisible)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Price = p.Price,
                        Discount = p.Discount,
                        StockQuantity = p.StockQuantity
                    })
                    .ToListAsync();

                // CREATE PAGINATION RESPONSE
                var paginationResponse = new
                {
                    Status = "Success",
                    TotalCount = totalInactiveProducts,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Products = inactiveProducts
                };

                return new OkObjectResult(new { Status = "Success", Body = paginationResponse });
            }
            catch (Exception)
            {
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while retrieving inactive products. Please try again later." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<IActionResult> UpdateProductStatusAsync(string productID, bool status)
        {
            try
            {
                // FIND THE PRODUCT BY ID
                var product = await _context.Products.FindAsync(productID);

                // CHECK IF PRODUCT EXISTS
                if (product == null)
                {
                    return new NotFoundObjectResult(new { Status = "Error", Message = "Product not found. Please check the provided ID." });
                }

                // UPDATE THE PRODUCT STATUS
                product.IsVisible = status;
                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                // CREATE A SUCCESS MESSAGE BASED ON THE STATUS
                string message = status ? "Product status updated successfully to active." : "Product status updated successfully to inactive.";

                return new OkObjectResult(new { Status = "Success", Message = message });
            }
            catch (Exception)
            {
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while updating the product status. Please try again later." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
