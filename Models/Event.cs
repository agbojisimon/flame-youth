namespace GlobalFlameMinistry.API.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Module { get; set; } = "Ministry";
        public bool IsCancelled { get; set; } = false;
        public bool AcceptsRegistrations { get; set; } = true;
        public bool AcceptsDonations { get; set; } = true;
        public string? DonationLabel { get; set; }
        public int? MinistryId { get; set; }
        public MinistryDepartment? Ministry { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}