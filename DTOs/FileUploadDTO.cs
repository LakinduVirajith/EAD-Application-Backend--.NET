using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EAD_Backend_Application__.NET.DTOs
{
    public class FileUploadDTO 
    {
        public string? FileName { get; set; }
        public required IFormFile File { get; set; }
    }
}
