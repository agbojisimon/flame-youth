namespace GlobalFlameMinistry.API.DTOs.Donation
{
    public class DonationStatsDto
    {
        public decimal GrandTotal { get; set; }
        public IEnumerable<DonationGroupDto> ByType { get; set; } = [];
        public IEnumerable<DonationGroupDto> ByMethod { get; set; } = [];
        public IEnumerable<DonationGroupDto> ByCurrency { get; set; } = [];
    }

    public class DonationGroupDto
    {
        public string GroupKey { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int Count { get; set; }
    }
}