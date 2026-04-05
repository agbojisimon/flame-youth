using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Donation
{
    public class InitiateDonationDto
    {
        [Required]
        public string DonorName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string DonorEmail { get; set; } = string.Empty;

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "NGN";

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        public string DonationType { get; set; } = string.Empty;

        public int? EventId { get; set; }
        public string? EventTitle { get; set; }
    }
}