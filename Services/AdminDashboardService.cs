using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Admin;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Interfaces.Admin;
using GlobalFlameMinistry.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Services.Admin
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IDonationRepository _donationRepo;

        public AdminDashboardService(AppDbContext context, UserManager<AppUser> userManager, IDonationRepository donationRepo)
        {
            _context = context;
            _userManager = userManager;
            _donationRepo = donationRepo;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            // USERS 
            var totalUsers = await _userManager.Users.CountAsync();
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var members = await _userManager.GetUsersInRoleAsync("Member");
            var youthMembers = await _userManager.GetUsersInRoleAsync("YouthMember");

            // ANNOUNCEMENTS 
            var totalAnnouncements = await _context.Announcements.CountAsync();
            var publishedAnnouncements = await _context.Announcements
                .CountAsync(a => a.IsPublished);
            var draftAnnouncements = await _context.Announcements
                .CountAsync(a => !a.IsPublished);
            var ministryAnnouncements = await _context.Announcements
                .CountAsync(a => a.Module == "Ministry");
            var youthAnnouncements = await _context.Announcements
                .CountAsync(a => a.Module == "Youth");

            // EVENTS 
            var totalEvents = await _context.Events.CountAsync();
            var upcomingEvents = await _context.Events
                .CountAsync(e => e.StartDate >= DateTime.UtcNow && !e.IsCancelled);
            var cancelledEvents = await _context.Events
                .CountAsync(e => e.IsCancelled);
            var ministryEvents = await _context.Events
                .CountAsync(e => e.Module == "Ministry");
            var youthEvents = await _context.Events
                .CountAsync(e => e.Module == "Youth");

            // ─── EVENT REGISTRATIONS
            var totalEventRegistrations = await _context.EventRegistrations.CountAsync();

            // PRAYER REQUESTS 
            var totalPrayerRequests = await _context.PrayerRequests.CountAsync();
            var pendingPrayerRequests = await _context.PrayerRequests
                .CountAsync(p => !p.IsAttendedTo);
            var attendedPrayerRequests = await _context.PrayerRequests
                .CountAsync(p => p.IsAttendedTo);

            // TESTIMONIES 
            var totalTestimonies = await _context.Testimonies.CountAsync();
            var pendingTestimonies = await _context.Testimonies
                .CountAsync(t => t.Status == TestimonyStatus.Pending);
            var approvedTestimonies = await _context.Testimonies
                .CountAsync(t => t.Status == TestimonyStatus.Approved);
            var rejectedTestimonies = await _context.Testimonies
                .CountAsync(t => t.Status == TestimonyStatus.Rejected);

            // CONTACTS 
            var totalContacts = await _context.Contacts.CountAsync();
            var newContacts = await _context.Contacts
                .CountAsync(c => c.Status == ContactMessageStatus.New);
            var readContacts = await _context.Contacts
                .CountAsync(c => c.Status == ContactMessageStatus.Read);
            var respondedContacts = await _context.Contacts
                .CountAsync(c => c.Status == ContactMessageStatus.Responded);
            var closedContacts = await _context.Contacts
                .CountAsync(c => c.Status == ContactMessageStatus.Closed);

            // SERMONS
            var totalSermons = await _context.Sermons.CountAsync();
            var publishedSermons = await _context.Sermons
                .CountAsync(s => s.IsPublished);
            var draftSermons = await _context.Sermons
                .CountAsync(s => !s.IsPublished);

            // DONATIONS
            var (totalAmount, completedDonations, pendingDonations) =
                await _donationRepo.GetSummaryAsync();

            // BOOKS
            var totalBooks = await _context.Books.CountAsync();
            var publishedBooks = await _context.Books.CountAsync(b => b.IsPublished);
            var draftBooks = await _context.Books.CountAsync(b => !b.IsPublished);
            var featuredBooks = await _context.Books.CountAsync(b => b.IsFeatured);

            return new DashboardStatsDto
            {
                // Users
                TotalUsers = totalUsers,
                TotalAdmins = admins.Count,
                TotalMembers = members.Count,
                TotalYouthMembers = youthMembers.Count,

                // Announcements
                TotalAnnouncements = totalAnnouncements,
                PublishedAnnouncements = publishedAnnouncements,
                DraftAnnouncements = draftAnnouncements,
                MinistryAnnouncements = ministryAnnouncements,
                YouthAnnouncements = youthAnnouncements,

                // Events
                TotalEvents = totalEvents,
                UpcomingEvents = upcomingEvents,
                CancelledEvents = cancelledEvents,
                MinistryEvents = ministryEvents,
                YouthEvents = youthEvents,

                // Events Registrationo
                TotalEventRegistrations = totalEventRegistrations,

                // Prayer Requests
                TotalPrayerRequests = totalPrayerRequests,
                PendingPrayerRequests = pendingPrayerRequests,
                AttendedPrayerRequests = attendedPrayerRequests,

                // Testimonies
                TotalTestimonies = totalTestimonies,
                PendingTestimonies = pendingTestimonies,
                ApprovedTestimonies = approvedTestimonies,
                RejectedTestimonies = rejectedTestimonies,

                // Contacts
                TotalContacts = totalContacts,
                NewContacts = newContacts,
                ReadContacts = readContacts,
                RespondedContacts = respondedContacts,
                ClosedContacts = closedContacts,

                // Sermons
                TotalSermons = totalSermons,
                PublishedSermons = publishedSermons,
                DraftSermons = draftSermons,

                // Donations
                TotalAmountReceived = totalAmount,
                CompletedDonations = completedDonations,
                PendingDonations = pendingDonations,

                // Books
                TotalBooks = totalBooks,
                PublishedBooks = publishedBooks,
                DraftBooks = draftBooks,
                FeaturedBooks = featuredBooks,
            };
        }
    }
}