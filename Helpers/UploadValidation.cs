namespace GlobalFlameMinistry.API.Helpers
{
    public static class UploadValidation
    {
        public const long MaxImageSizeBytes = 10 * 1024 * 1024;
        public const long MaxAudioSizeBytes = 100 * 1024 * 1024;
        public const long MaxPdfSizeBytes = 10 * 1024 * 1024;

        private static readonly string[] AllowedImageTypes =
            { "image/jpeg", "image/png", "image/webp", "image/gif" };

        public static bool IsAllowedContentType(string? contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) return false;

            if (AllowedImageTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
                return true;

            if (contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
                return true;

            if (contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                return true;

            return contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase);
        }

        public static long GetMaxSizeForContentType(string? contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) return 0;

            if (AllowedImageTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
                return MaxImageSizeBytes;

            if (contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
                return MaxAudioSizeBytes;

            if (contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                return MaxAudioSizeBytes;

            if (contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                return MaxPdfSizeBytes;

            return 0;
        }
    }
}