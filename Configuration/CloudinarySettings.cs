namespace GlobalFlameMinistry.API.Configuration
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string UploadPreset { get; set; } = "gfm_uploads";
    }
}