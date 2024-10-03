using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend_Application__.NET.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        // GET: api/v1
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new { message = "Application is running well!" });
        }
    }
}
