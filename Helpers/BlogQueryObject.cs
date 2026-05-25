using GlobalFlameMinistry.API.Helpers.Queries;

namespace GlobalFlameMinistry.API.Helpers
{
    public class BlogQueryObject : BaseQueryObject
    {
        public string? Department { get; set; }
        public bool? IsPublished { get; set; }
        public string? SearchTerm { get; set; }
    }
}
