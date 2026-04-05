using GlobalFlameMinistry.API.Helpers.Queries;

namespace GlobalFlameMinistry.API.Helpers
{
    public class BookQueryObject : BaseQueryObject
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public bool? IsPublished { get; set; }
        public bool? IsFeatured { get; set; }
    }
}