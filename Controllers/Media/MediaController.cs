using System.Security.Cryptography;
using System.Text;
using GlobalFlameMinistry.API.DTOs.Media;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Media
{
    public class SignUploadRequest
    {
        public string? Folder { get; set; }
        public string? PublicId { get; set; }
    }

    [Route("api/media")]
    [ApiController]
    [Authorize]
    public class MediaController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ICloudinaryService _cloudinaryService;

        public MediaController(
            IConfiguration config,
            ICloudinaryService cloudinaryService)
        {
            _config = config;
            _cloudinaryService = cloudinaryService;
        }

        // POST /api/media/upload
        // Multipart form-data: file (required), folder (optional), publicId (optional)
        [HttpPost("upload")]
        [RequestFormLimits(
            MultipartBodyLengthLimit = 120 * 1024 * 1024,
            ValueLengthLimit = 4096)]
        [RequestSizeLimit(120 * 1024 * 1024)]
        public async Task<IActionResult> Upload([FromForm] MediaUploadRequest request)
        {
            if (!UploadValidation.IsAllowedContentType(request.File.ContentType))
                return BadRequest("Unsupported file type.");

            if (request.File.Length > UploadValidation.GetMaxSizeForContentType(request.File.ContentType))
                return BadRequest("File is too large.");

            try
            {
                var result = await _cloudinaryService.UploadAsync(
                    request.File,
                    request.Folder,
                    request.PublicId);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(502, new { message = ex.Message });
            }
        }

        [HttpPost("sign-upload")]
        public IActionResult SignUpload([FromBody] SignUploadRequest? request)
        {
            var cloudName = _config["Cloudinary:CloudName"];
            var apiKey = _config["Cloudinary:ApiKey"];
            var apiSecret = _config["Cloudinary:ApiSecret"];

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                return StatusCode(500, new { message = "Cloudinary is not configured." });
            }

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var parameters = new SortedDictionary<string, string>
            {
                ["timestamp"] = timestamp.ToString()
            };

            if (!string.IsNullOrWhiteSpace(request?.Folder))
                parameters["folder"] = request.Folder;

            if (!string.IsNullOrWhiteSpace(request?.PublicId))
                parameters["public_id"] = request.PublicId;

            var signString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}")) + apiSecret;

            var signature = Convert.ToHexString(
                SHA1.HashData(Encoding.UTF8.GetBytes(signString)))
                .ToLowerInvariant();

            return Ok(new
            {
                signature,
                timestamp,
                cloudName,
                apiKey,
                folder = request?.Folder
            });
        }
    }
}
