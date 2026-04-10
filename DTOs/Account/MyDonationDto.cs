namespace GlobalFlameMinistry.API.DTOs.Account
{
    public class MyDonationDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string DonationType { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public int? EventId { get; set; }
        public string? EventTitle { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}