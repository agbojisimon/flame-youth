public class EventResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Module { get; set; } = string.Empty;
    public bool IsCancelled { get; set; }
    public bool AcceptsRegistrations { get; set; }
    public bool AcceptsDonations { get; set; }
    public string? DonationLabel { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}