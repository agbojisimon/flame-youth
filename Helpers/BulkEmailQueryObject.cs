using GlobalFlameMinistry.API.Helpers.Queries;

namespace GlobalFlameMinistry.API.Helpers
{
    public class BulkEmailQueryObject : BaseQueryObject
    {
        public string? Subject { get; set; }
        public string? Status { get; set; }
        public string? TargetGroup { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}