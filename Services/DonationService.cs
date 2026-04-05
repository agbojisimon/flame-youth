using System.Text;
using System.Text.Json;
using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Donation;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Services
{
    public class DonationService : IDonationService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public DonationService(
            AppDbContext context,
            HttpClient httpClient,
            IConfiguration config)
        {
            _context = context;
            _httpClient = httpClient;
            _config = config;
        }

        // ── PAYSTACK ──────────────────────────────────────────────────────────
        public async Task<InitiateDonationResponseDto> InitiatePaystackAsync(
            InitiateDonationDto dto)
        {
            var secretKey = _config["Paystack:SecretKey"];
            var callbackUrl = _config["Paystack:CallbackUrl"];
            var reference = GenerateReference("PSK");

            // Subaccounts are a Paystack-only concept
            var subaccountCode = PaystackSubaccounts.GetSubaccount(dto.DonationType);

            // Save pending donation — pass subaccount only for Paystack
            var donation = await SaveDonationAsync(dto, reference, "Paystack", subaccountCode);

            var payload = new Dictionary<string, object>
            {
                ["email"] = dto.DonorEmail,
                ["amount"] = (int)(dto.Amount * 100), // Paystack uses kobo
                ["currency"] = dto.Currency,
                ["reference"] = reference,
                ["callback_url"] = callbackUrl!,
                ["metadata"] = new
                {
                    donor_name = dto.DonorName,
                    donation_type = dto.DonationType,
                    donation_id = donation.Id,
                    custom_fields = new[]
                    {
                        new { display_name = "Donor Name",    variable_name = "donor_name",    value = dto.DonorName },
                        new { display_name = "Donation Type", variable_name = "donation_type", value = dto.DonationType },
                    }
                }
            };

            if (!string.IsNullOrEmpty(subaccountCode) &&
                !subaccountCode.Contains("placeholder"))
            {
                payload["subaccount"] = subaccountCode;
                payload["transaction_charge"] = 0; // Church gets 100%
                payload["bearer"] = "subaccount";
            }

            var json = JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://api.paystack.co/transaction/initialize");
            request.Headers.Add("Authorization", $"Bearer {secretKey}");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

            if (result.GetProperty("status").GetBoolean())
            {
                var paymentUrl = result
                    .GetProperty("data")
                    .GetProperty("authorization_url")
                    .GetString()!;

                return new InitiateDonationResponseDto
                {
                    PaymentUrl = paymentUrl,
                    Reference = reference
                };
            }

            throw new ApplicationException(
                result.GetProperty("message").GetString()
                ?? "Failed to initialize payment");
        }

        // ── FLUTTERWAVE ───────────────────────────────────────────────────────
        public async Task<InitiateDonationResponseDto> InitiateFlutterwaveAsync(
            InitiateDonationDto dto)
        {
            var secretKey = _config["Flutterwave:SecretKey"];
            var callbackUrl = _config["Flutterwave:CallbackUrl"];
            var reference = GenerateReference("FLW");

            // Flutterwave has no subaccount concept — don't pass one
            var donation = await SaveDonationAsync(dto, reference, "Flutterwave");

            var payload = new
            {
                tx_ref = reference,
                amount = dto.Amount,
                currency = dto.Currency,
                redirect_url = callbackUrl,
                customer = new
                {
                    email = dto.DonorEmail,
                    name = dto.DonorName,
                },
                customizations = new
                {
                    title = "Global Flame Ministries",
                    description = $"Donation — {dto.DonationType}",
                    logo = "https://globalflameministries.org/logo.png"
                },
                meta = new
                {
                    donation_type = dto.DonationType,
                    donation_id = donation.Id,
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://api.flutterwave.com/v3/payments");
            request.Headers.Add("Authorization", $"Bearer {secretKey}");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

            if (result.GetProperty("status").GetString() == "success")
            {
                var paymentUrl = result
                    .GetProperty("data")
                    .GetProperty("link")
                    .GetString()!;

                return new InitiateDonationResponseDto
                {
                    PaymentUrl = paymentUrl,
                    Reference = reference
                };
            }

            throw new ApplicationException(
                result.GetProperty("message").GetString()
                ?? "Failed to initialize payment");
        }

        // ── PAYSTACK VERIFICATION ─────────────────────────────────────────────
        public async Task<bool> VerifyPaystackAsync(string reference)
        {
            var secretKey = _config["Paystack:SecretKey"];

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://api.paystack.co/transaction/verify/{reference}");
            request.Headers.Add("Authorization", $"Bearer {secretKey}");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

            if (!result.GetProperty("status").GetBoolean())
                return false;

            var data = result.GetProperty("data");
            var status = data.GetProperty("status").GetString();

            // ✅ Fixed: was .FirstOrDefault() — must be async
            var donation = await _context.Donations
                .FirstOrDefaultAsync(d => d.TransactionReference == reference);

            if (donation is not null)
            {
                donation.Status = status == "success" ? "Completed" : "Failed";
                donation.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return status == "success";
        }

        // ── FLUTTERWAVE VERIFICATION ──────────────────────────────────────────
        public async Task<bool> VerifyFlutterwaveAsync(string transactionId)
        {
            var secretKey = _config["Flutterwave:SecretKey"];

            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://api.flutterwave.com/v3/transactions/{transactionId}/verify");
            request.Headers.Add("Authorization", $"Bearer {secretKey}");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

            if (result.GetProperty("status").GetString() != "success")
                return false;

            var data = result.GetProperty("data");
            var status = data.GetProperty("status").GetString();
            var txRef = data.GetProperty("tx_ref").GetString();

            // ✅ Fixed: was .FirstOrDefault() — must be async
            var donation = await _context.Donations
                .FirstOrDefaultAsync(d => d.TransactionReference == txRef);

            if (donation is not null)
            {
                donation.Status = status == "successful" ? "Completed" : "Failed";
                donation.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return status == "successful";
        }

        // ── PRIVATE HELPERS ───────────────────────────────────────────────────
        private async Task<Donation> SaveDonationAsync(
            InitiateDonationDto dto,
            string reference,
            string method,
            string? subaccountCode = null) // null by default — only Paystack passes this
        {
            var donation = new Donation
            {
                DonorName = dto.DonorName,
                DonorEmail = dto.DonorEmail,
                Amount = dto.Amount,
                Currency = dto.Currency,
                TransactionReference = reference,
                PaymentMethod = method,
                Status = "Pending",
                DonationType = dto.DonationType,
                SubaccountCode = subaccountCode,  // null for Flutterwave
                EventId = dto.EventId,
                EventTitle = dto.EventTitle,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Donations.AddAsync(donation);
            await _context.SaveChangesAsync();
            return donation;
        }

        private static string GenerateReference(string prefix)
        {
            // Extended GUID slice from 8 → 12 for better uniqueness under load
            return $"{prefix}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..12].ToUpper()}";
        }
    }
}