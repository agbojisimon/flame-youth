namespace GlobalFlameMinistry.API.DTOs.Media
{
    public class MediaUploadResponseDto
    {
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public double? Duration { get; set; }
        public long? Bytes { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? PageCount { get; set; }
    }
}