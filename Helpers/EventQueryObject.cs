using g_flame_youth.Helpers.Queries;

namespace g_flame_youth.Helpers
{
    public class EventQueryObject : BaseQueryObject
    {
        public string? Title { get; set; } = null;
        public string? Description { get; set; } = null;
    }
}