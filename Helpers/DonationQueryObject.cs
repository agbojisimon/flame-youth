namespace GlobalFlameMinistry.API.Helpers.Queries
{
    public class DonationQueryObject : BaseQueryObject
    {
        public string? DonorName { get; set; }
        public string? DonorEmail { get; set; }
        public string? Status { get; set; }
        public string? DonationType { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Currency { get; set; }
        public int? EventId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}