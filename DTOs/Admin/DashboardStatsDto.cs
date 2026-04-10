namespace GlobalFlameMinistry.API.DTOs.Admin
{
    public class DashboardStatsDto
    {
        // USERS
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalMembers { get; set; }
        public int TotalYouthMembers { get; set; }

        // ANNOUNCEMENTS
        public int TotalAnnouncements { get; set; }
        public int PublishedAnnouncements { get; set; }
        public int DraftAnnouncements { get; set; }
        public int MinistryAnnouncements { get; set; }
        public int YouthAnnouncements { get; set; }

        // EVENTS
        public int TotalEvents { get; set; }
        public int UpcomingEvents { get; set; }
        public int CancelledEvents { get; set; }
        public int MinistryEvents { get; set; }
        public int YouthEvents { get; set; }

        // PRAYER REQUESTS
        public int TotalPrayerRequests { get; set; }
        public int PendingPrayerRequests { get; set; }
        public int AttendedPrayerRequests { get; set; }

        // TESTIMONIES
        public int TotalTestimonies { get; set; }
        public int PendingTestimonies { get; set; }
        public int ApprovedTestimonies { get; set; }
        public int RejectedTestimonies { get; set; }

        // CONTACTS 
        public int TotalContacts { get; set; }
        public int NewContacts { get; set; }
        public int ReadContacts { get; set; }
        public int RespondedContacts { get; set; }
        public int ClosedContacts { get; set; }

        // EVENTS REGISTRATION
        public int TotalEventRegistrations { get; set; }

        // SERMONS
        public int TotalSermons { get; set; }
        public int PublishedSermons { get; set; }
        public int DraftSermons { get; set; }

        // DONATIONS
        public decimal TotalAmountReceived { get; set; }
        public int CompletedDonations { get; set; }
        public int PendingDonations { get; set; }

        // BOOKS
        public int TotalBooks { get; set; }
        public int PublishedBooks { get; set; }
        public int DraftBooks { get; set; }
        public int FeaturedBooks { get; set; }
    }
}