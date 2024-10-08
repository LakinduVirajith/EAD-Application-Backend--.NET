using EAD_Backend_Application__.NET.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Services
{
    public interface IRankingService
    {
        Task<IActionResult> AddVendorRankingAsync(RankingDTO dto);
        Task<IActionResult> GetVendorRatingAvarageAsync(string vendorId);
        Task<IActionResult> GetVendorTotalSalesAsync(string vendorId);
        Task<ActionResult<IEnumerable<RankingDetailsDTO>>> GetVendorRankingsAsync(string email);
    }
}
