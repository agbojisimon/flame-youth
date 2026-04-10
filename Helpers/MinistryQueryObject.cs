using GlobalFlameMinistry.API.Helpers.Queries;

namespace GlobalFlameMinistry.API.Helpers
{
    public class MinistryQueryObject : BaseQueryObject
    {
        public string? Name { get; set; }
        public bool? IsPublished { get; set; }
    }
}