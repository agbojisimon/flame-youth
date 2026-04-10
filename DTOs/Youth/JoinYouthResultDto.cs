namespace GlobalFlameMinistry.API.DTOs.Youth
{
    public class JoinYouthResultDto
    {
        public bool AutoJoined { get; set; }
        public bool RequiresVerification { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}