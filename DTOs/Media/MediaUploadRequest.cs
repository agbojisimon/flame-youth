using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Media
{
    public class MediaUploadRequest
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        public string? Folder { get; set; }

        public string? PublicId { get; set; }
    }
}