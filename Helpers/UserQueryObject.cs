using GlobalFlameMinistry.API.Helpers.Queries;

namespace GlobalFlameMinistry.API.Helpers
{
    public class UserQueryObject : BaseQueryObject
    {
        public string? Email { get; set; } = null;
        public string? FullName { get; set; } = null;
    }
}