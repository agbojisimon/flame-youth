using g_flame_youth.Helpers.Queries;

namespace g_flame_youth.Helpers
{
    public class UserQueryObject : BaseQueryObject
    {
        public string? Email { get; set; } = null;
        public string? FullName { get; set; } = null;
    }
}