using GlobalFlameMinistry.API.Helpers.Queries;

namespace GlobalFlameMinistry.API.Helpers
{
    public class PrayerRequestQueryObject : BaseQueryObject
    {
        public string? Name { get; set; }

        public bool? IsAttendedTo { get; set; }
        // Date range filters
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}