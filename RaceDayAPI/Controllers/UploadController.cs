using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RaceDayAPI.Helpers;
using RaceDayAPI.Services;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IAzureStorageService _storageService;

        // Inject the Azure service we just created
        public UploadController(IAzureStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("profile")]
        [RequireAuth]
        public async Task<IActionResult> UploadProfile(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest(new { error = "No file uploaded." });
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant())) return BadRequest(new { error = "Invalid file type." });
            if (file.Length > 5 * 1024 * 1024) return BadRequest(new { error = "File size exceeds the 5MB limit." });

            var url = await _storageService.UploadFileAsync(file, "profile-images");
            return Ok(new { url });
        }

        // Endpoint to upload Event Banners
        [HttpPost("banner")]
        [RequireAuth] // MVC uses this endpoint for both organiser banners and participant profile pictures
        public async Task<IActionResult> UploadBanner(IFormFile file)
        {
            // 1. Basic Null Check
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded." });

            // 2. Validate File Type (Only allow images)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { error = "Invalid file type. Only image files are allowed." });

            // 3. Validate File Size (Max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { error = "File size exceeds the 5MB limit." });

            // 4. Upload to Azure and return the URL
            try
            {
                // We tell the service to put it in the "event-banners" folder in Azure
                var url = await _storageService.UploadFileAsync(file, "event-banners");
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to upload file to Azure.", detail = ex.Message });
            }
        }
    }
}