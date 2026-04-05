namespace GlobalFlameMinistry.API.DTOs.Donation
{
    public class DonationResponseDto
    {
        public int Id { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public string DonorEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DonationType { get; set; } = string.Empty;
        public string? SubaccountCode { get; set; }
        public int? EventId { get; set; }
        public string? EventTitle { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class InitiateDonationResponseDto
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }
}