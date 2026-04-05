namespace GlobalFlameMinistry.API.Models
{
  public class Donation
  {
    public int Id { get; set; }
    public string DonorName { get; set; } = string.Empty;
    public string DonorEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "NGN";
    public string TransactionReference { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string DonationType { get; set; } = string.Empty;
    public string? SubaccountCode { get; set; }
    public int? EventId { get; set; }
    public string? EventTitle { get; set; }
    public string? AppUserId { get; set; }
    public AppUser? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
  }
}