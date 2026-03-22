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
    public string Status { get; set; } = "Pending";
    public string Module { get; set; } = "Ministry";
    public string? AppUserId { get; set; }
    public AppUser? User { get; set; }
    public DateTime DonatedAt { get; set; } = DateTime.UtcNow;
  }
}