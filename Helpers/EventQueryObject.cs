using GlobalFlameMinistry.API.Helpers.Queries;
using Microsoft.AspNetCore.Components.Web;

namespace GlobalFlameMinistry.API.Helpers
{
    public class EventQueryObject : BaseQueryObject
    {
        public string? Title { get; set; }
        public string? Module { get; set; }
        public string? Location { get; set; }
        public bool? IsCancelled { get; set; }
        public bool? UpcomingOnly { get; set; }
        public bool? OngoingOnly { get; set; }
        public bool? PastOnly { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}