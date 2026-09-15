using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using GlobalFlameMinistry.API.Configuration;
using GlobalFlameMinistry.API.DTOs.Media;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.Media;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace GlobalFlameMinistry.API.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinarySettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(
            IOptions<CloudinarySettings> settings,
            HttpClient httpClient,
            ILogger<CloudinaryService> logger)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<MediaUploadResponseDto> UploadAsync(
            IFormFile file,
            string? folder,
            string? publicId)
        {
            if (string.IsNullOrWhiteSpace(_settings.CloudName) ||
                string.IsNullOrWhiteSpace(_settings.ApiKey) ||
                string.IsNullOrWhiteSpace(_settings.ApiSecret))
            {
                throw new InvalidOperationException("Cloudinary is not configured.");
            }

            var contentType = file.ContentType;
            if (!UploadValidation.IsAllowedContentType(contentType))
                throw new InvalidOperationException("Unsupported file type.");

            if (file.Length > UploadValidation.GetMaxSizeForContentType(contentType))
                throw new InvalidOperationException("File is too large.");

            var resourceType = ResolveResourceType(contentType);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var safeFileName = SanitizeFileName(file.FileName);

            // Cloudinary signs every posted field except file/api_key/resource_type/cloud_name
            var parameters = new SortedDictionary<string, string>
            {
                ["timestamp"] = timestamp
            };

            if (!string.IsNullOrWhiteSpace(folder))
                parameters["folder"] = folder;

            if (!string.IsNullOrWhiteSpace(publicId))
                parameters["public_id"] = publicId;

            var signString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}")) + _settings.ApiSecret;
            var signature = Convert.ToHexString(
                    SHA1.HashData(Encoding.UTF8.GetBytes(signString)))
                .ToLowerInvariant();

            using var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(fileContent, "file", safeFileName);

            content.Add(new StringContent(_settings.ApiKey), "api_key");

            foreach (var parameter in parameters)
                content.Add(new StringContent(parameter.Value), parameter.Key);

            content.Add(new StringContent(signature), "signature");

            var uploadUrl = $"https://api.cloudinary.com/v1_1/{_settings.CloudName}/{resourceType}/upload";

            try
            {
                using var response = await _httpClient.PostAsync(uploadUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "[Cloudinary] Upload failed. Status: {Status} | Cloud: {Cloud} | Response: {Body}",
                        (int)response.StatusCode,
                        _settings.CloudName,
                        Truncate(responseBody, 500));

                    throw new InvalidOperationException("Upload failed. Please try again.");
                }

                var json = JObject.Parse(responseBody);

                var result = new MediaUploadResponseDto
                {
                    Url = json["secure_url"]?.Value<string>() ?? string.Empty,
                    PublicId = json["public_id"]?.Value<string>() ?? string.Empty
                };

                if (string.IsNullOrWhiteSpace(result.Url))
                {
                    _logger.LogWarning(
                        "[Cloudinary] Upload succeeded but secure_url was missing. Cloud: {Cloud}",
                        _settings.CloudName);

                    throw new InvalidOperationException("Upload failed. Please try again.");
                }

                _logger.LogInformation(
                    "[Cloudinary] Upload successful. PublicId: {PublicId} | Cloud: {Cloud}",
                    result.PublicId,
                    _settings.CloudName);

                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(
                    ex,
                    "[Cloudinary] Network error while uploading to cloud {Cloud}.",
                    _settings.CloudName);

                throw new InvalidOperationException("Upload failed. Please try again.");
            }
        }

        private static string ResolveResourceType(string contentType)
        {
            if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return "image";

            if (contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
                return "video"; // Cloudinary stores audio as video

            if (contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                return "video";

            return "raw"; // PDF and other documents
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "file";

            var invalid = Path.GetInvalidFileNameChars()
                .Concat(new[] { '\\', '/' })
                .ToHashSet();

            var sanitized = new string(fileName
                .Select(c => invalid.Contains(c) ? '_' : c)
                .ToArray());

            if (string.IsNullOrWhiteSpace(sanitized))
                return "file";

            return sanitized.Length > 100 ? sanitized[..100] : sanitized;
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Length <= maxLength ? value : value[..maxLength];
        }
    }
}