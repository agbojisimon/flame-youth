using GlobalFlameMinistry.API.Helpers.Queries;

namespace GlobalFlameMinistry.API.Helpers
{
    public class AnnouncementQueryObject : BaseQueryObject
    {
        public string? Title { get; set; }
        public string? Module { get; set; }
        public string? Category { get; set; }
        public bool? IsPublished { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}