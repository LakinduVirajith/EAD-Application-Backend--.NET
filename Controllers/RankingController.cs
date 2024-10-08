using EAD_Backend_Application__.NET.DTOs;
using EAD_Backend_Application__.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Controllers
{
    [Route("api/v1/ranking")]
    [ApiController]
    public class RankingController
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        /// <summary>Adds a ranking and review for a vendor based on a customer's order.</summary>
        // POST: api/v1/ranking/vendor
        [HttpPost("vendor")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddVendorRanking(RankingDTO dto)
        {
            return await _rankingService.AddVendorRankingAsync(dto);
        }

        /// <summary>Retrieves the average rating for a specified vendor based on customer reviews.</summary>
        // GET: api/v1/ranking/average/{vendorId}
        [HttpGet("average/{vendorId}")]
        [Authorize]
        public async Task<IActionResult> GetVendorRatingAvarage(string vendorId)
        {
            return await _rankingService.GetVendorRatingAvarageAsync(vendorId);
        }

        /// <summary>Retrieves the total sales for a specified vendor.</summary>
        // GET: api/v1/ranking/sales/{vendorId}
        [HttpGet("sales/{vendorId}")]
        [Authorize(Roles = "Admin, CSR, Vendor")]
        public async Task<IActionResult> GetVendorTotalSales(string vendorId)
        {
            return await _rankingService.GetVendorTotalSalesAsync(vendorId);
        }

        /// <summary>Retrieves detailed ranking and review information for a specified vendor.</summary>
        // GET: api/v1/ranking/details/{email}
        [HttpGet("details/{email}")]
        [Authorize(Roles = "Admin, CSR, Vendor")]
        public async Task<ActionResult<IEnumerable<RankingDetailsDTO>>> GetVendorRankings(string email)
        {
            return await _rankingService.GetVendorRankingsAsync(email);
        }
    }
}
