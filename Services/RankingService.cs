using EAD_Backend_Application__.NET.Data;
using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Enums;
using EAD_Backend_Application__.NET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Security.Claims;

namespace EAD_Backend_Application__.NET.Services
{
    public class RankingService : IRankingService
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public RankingService(UserManager<UserModel> userManager, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<IActionResult> AddVendorRankingAsync(RankingDTO dto)
        {
            // GET THE EMAIL FROM THE AUTHENTICATION HEADER
            var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;


            // CHECK IF EMAIL IS NULL
            if (string.IsNullOrEmpty(email))
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "User not found. Please ensure you are logged in." });
            }
            // FIND THE USER BY EMAIL AND IF USER IS NULL
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "User not found. Please ensure you are logged in." });
            }

            // MAP DTO TO RANKING MODEL
            var ranking = new RankingModel
            {
                Comment = dto.Comment,
                Rating = dto.Rating,
                CustomerId = user.Id,
                CreatedAt = dto.CreatedAt,
                VendorId = dto.VendorId
            };

            // SAVE TO DATABASE (USING DBCONTEXT)
            try
            {
                _context.Rankings.Add(ranking);
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { Status = "Success", Message = "Ranking successfully saved to the database." });
            }
            catch (Exception)
            {
                return new ObjectResult(new { Status = "Error", Message = "An error occurred while saving the ranking. Please try again later." })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<IActionResult> GetVendorRatingAvarageAsync(string vendorId)
        {
            // VALIDATE IF VENDOR EXISTS
            var vendorExists = await _context.Users.AnyAsync(u => u.Id == vendorId && u.Role == "Vendor");
            if (!vendorExists)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "The specified vendor could not be found. Please check the vendor ID and try again." });
            }

            // CALCULATE THE AVERAGE RATING FOR THE VENDOR
            var averageRating = await _context.Rankings
                .Where(r => r.VendorId == vendorId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;

            // RETURN AVERAGE RATING
            return new OkObjectResult(new { Status = "Success", Body = averageRating });
        }

        public async Task<IActionResult> GetVendorTotalSalesAsync(string vendorId)
        {
            // VALIDATE IF VENDOR EXISTS
            var vendorExists = await _context.Users.AnyAsync(u => u.Id == vendorId && u.Role == "Vendor");
            if (!vendorExists)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "The specified vendor could not be found. Please check the vendor ID and try again." });
            }

            // CALCULATE THE TOTAL SALES FOR THE VENDOR
            var totalSales = await _context.Orders
                .Where(o => o.Status == OrderStatus.Completed.ToString()) // ONLY COMPLETE ORDERS
                .SelectMany(o => o.OrderItems) // FLATTEN THE LIST OF ORDER ITEMS ACROSS ALL ORDERS
                .Where(oi => _context.Products.Any(p => p.ProductId == oi.ProductId && p.VendorId == vendorId)) // FILTER BY VENDOR'S PRODUCT
                .SumAsync(oi => oi.Price * oi.Quantity); // CALCULATE TOTAL SALES FOR VENDOR'S PRODUCTS

            // RETURN TOTAL SALES
            return new OkObjectResult(new { Status = "Success", Body = totalSales });
        }

        public async Task<ActionResult<IEnumerable<RankingDetailsDTO>>> GetVendorRankingsAsync(string vendorId)
        {
            // VALIDATE IF VENDOR EXISTS
            var vendorExists = await _context.Users.AnyAsync(u => u.Id == vendorId && u.Role == "Vendor");
            if (!vendorExists)
            {
                return new NotFoundObjectResult(new { Status = "Error", Message = "The specified vendor could not be found. Please check the vendor ID and try again." });
            }

            // RETRIEVE RANKINGS FOR THE VENDOR
            var rankings = await _context.Rankings
                .Where(r => r.VendorId == vendorId)
                .Select(r => new RankingDetailsDTO
                {
                    VendorId = r.VendorId,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CustomerId = r.CustomerId,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            // RETURN RANKINGS
            return new OkObjectResult(new { Status = "Success", body = rankings });
        }
    }
}
