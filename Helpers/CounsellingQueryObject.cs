using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Helpers
{
    public class CounsellingQueryObject : BaseQueryObject
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Topic { get; set; }
        public CounsellingStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}