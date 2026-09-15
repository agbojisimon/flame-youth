using GlobalFlameMinistry.API.DTOs.Media;

namespace GlobalFlameMinistry.API.Interfaces.Media
{
    public interface ICloudinaryService
    {
        Task<MediaUploadResponseDto> UploadAsync(
            IFormFile file,
            string? folder,
            string? publicId);
    }
}