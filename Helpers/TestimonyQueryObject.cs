using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Helpers
{
    public class TestimonyQueryObject : BaseQueryObject
    {
        public string? FullName { get; set; }
        public TestimonyStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}