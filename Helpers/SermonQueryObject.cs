namespace GlobalFlameMinistry.API.Helpers.Queries
{
    public class SermonQueryObject : BaseQueryObject
    {
        public string? Title { get; set; }
        public string? Speaker { get; set; }
        public string? Series { get; set; }
        public bool? IsPublished { get; set; }
        public bool? IsFeatured { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}