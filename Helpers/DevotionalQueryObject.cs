
namespace g_flame_youth.Helpers.Queries
{
    public class DevotionalQueryObject : BaseQueryObject
    {
        public bool? IsPublished { get; set; } = null;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}