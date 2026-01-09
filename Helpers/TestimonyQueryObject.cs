using g_flame_youth.Helpers.Queries;

namespace g_flame_youth.Helpers
{
    public class TestimonyQueryObject : BaseQueryObject
    {
        public string? AppUserId { get; set; }

        public bool? Status { get; set; }

        public DateTime? CreatedAfter { get; set; }

        public DateTime? CreatedBefore { get; set; }

        public string? SearchTerm { get; set; }
    }
}