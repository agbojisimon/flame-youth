using System.Security.Cryptography;
using System.Text;
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

        public MediaController(IConfiguration config)
        {
            _config = config;
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
