using GlobalFlameMinistry.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        // Strongly typed options — tells EF Core exactly which DbContext this config belongs to
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        // ─── DB SETS ──────────────────────────────────────────────────────────
        // Each one = one table in SQL Server
        // Devotional is GONE — removed as requested
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
        public DbSet<PrayerRequest> PrayerRequests { get; set; }
        public DbSet<Testimony> Testimonies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Sermon> Sermons { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Sets up all Identity tables — always call this first
            base.OnModelCreating(builder);

            // ─── APP USER ─────────────────────────────────────────────────────
            // FullName is [NotMapped] so EF ignores it automatically
            // Just enforcing unique email at DB level as extra safety net
            builder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.RefreshToken).HasMaxLength(500);
                entity.Property(u => u.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            });

            // ─── ANNOUNCEMENT ─────────────────────────────────────────────────
            builder.Entity<Announcement>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Title).IsRequired().HasMaxLength(200);
                entity.Property(a => a.Content).IsRequired();
                entity.Property(a => a.CreatedById).IsRequired();
                // "Ministry" or "Youth"
                entity.Property(a => a.Module).IsRequired().HasMaxLength(50).HasDefaultValue("Ministry");
                entity.Property(a => a.Category).HasMaxLength(100);
                entity.Property(a => a.IsPublished).HasDefaultValue(false);
                entity.Property(a => a.IsDeleted).HasDefaultValue(false);
                entity.Property(a => a.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
                entity.HasQueryFilter(a => !a.IsDeleted);
            });

            // ─── EVENT ────────────────────────────────────────────────────────
            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.Location).IsRequired().HasMaxLength(300);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);
                entity.Property(e => e.Module).IsRequired().HasMaxLength(50).HasDefaultValue("Ministry");
                entity.Property(e => e.IsCancelled).HasDefaultValue(false);
                entity.Property(e => e.AcceptsRegistrations).HasDefaultValue(true);
                entity.Property(e => e.AcceptsDonations).HasDefaultValue(true);
                entity.Property(e => e.DonationLabel).HasMaxLength(200);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            builder.Entity<EventRegistration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.HasOne(e => e.Event)
                    .WithMany()
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── PRAYER REQUEST ───────────────────────────────────────────────
            builder.Entity<PrayerRequest>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).HasMaxLength(150);   // No .IsRequired()
                entity.Property(p => p.Email).HasMaxLength(200);   // No .IsRequired()
                entity.Property(p => p.Content).IsRequired().HasMaxLength(1000);
                entity.Property(p => p.Attachment).HasMaxLength(500);
                entity.Property(p => p.IsAttendedTo).HasDefaultValue(false);
                // AnonymousToken must be unique — each prayer request gets its own token
                entity.Property(p => p.AnonymousToken).IsRequired().HasMaxLength(100);
                entity.HasIndex(p => p.AnonymousToken).IsUnique();
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Optional relationship — logged-in users get linked, anonymous users don't
                // SetNull = if user account is deleted, prayer request stays but UserId = null
                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.AppUserId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });

            // ─── TESTIMONY ────────────────────────────────────────────────────
            builder.Entity<Testimony>(entity =>
            {
                entity.HasKey(t => t.Id);

                // Name is optional — anonymous users don't have to identify themselves
                entity.Property(t => t.FullName).HasMaxLength(150);

                entity.Property(t => t.Content).IsRequired().HasMaxLength(2000);
                entity.Property(t => t.Attachment).HasMaxLength(500);

                // Store enum as int — 0=Pending, 1=Approved, 2=Rejected
                entity.Property(t => t.Status)
                    .HasDefaultValue(TestimonyStatus.Pending)
                    .HasConversion<int>();

                entity.Property(t => t.IsDeleted).HasDefaultValue(false);
                entity.Property(t => t.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Optional relationship — SetNull because anonymous users have no account
                // If a linked user deletes their account, the testimony stays but AppUserId becomes null
                entity.HasOne(t => t.User)
                    .WithMany(u => u.Testimonies)
                    .HasForeignKey(t => t.AppUserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);

                entity.HasQueryFilter(t => !t.IsDeleted);
            });

            // ─── CONTACT ──────────────────────────────────────────────────────
            builder.Entity<Contact>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.FullName).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
                entity.Property(c => c.PhoneNumber).HasMaxLength(20);
                entity.Property(c => c.Message).IsRequired().HasColumnType("nvarchar(max)");
                // Enum stored as int in DB — 1=New, 2=Read, 3=Responded, 4=Closed
                entity.Property(c => c.Status).IsRequired().HasDefaultValue(ContactMessageStatus.New);
                // Enum stored as int — 1=General, 2=JoinRequest, 3=Counselling, 4=Feedback
                entity.Property(c => c.Type).IsRequired().HasDefaultValue(ContactMessageType.General);
                entity.Property(c => c.IsDeleted).HasDefaultValue(false);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                // Index for faster date-based queries on admin dashboard
                entity.HasIndex(c => c.CreatedAt);
                // Soft delete filter
                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            // ─── DONATION ─────────────────────────────────────────────────────
            builder.Entity<Donation>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.DonorName).IsRequired().HasMaxLength(150);
                entity.Property(d => d.DonorEmail).IsRequired().HasMaxLength(200);
                // decimal(18,2) = precise money storage — never use float for currency
                entity.Property(d => d.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(d => d.Currency).HasMaxLength(10).HasDefaultValue("NGN");
                // Unique — each payment gateway transaction has one unique reference
                entity.Property(d => d.TransactionReference).IsRequired().HasMaxLength(200);
                entity.HasIndex(d => d.TransactionReference).IsUnique();
                entity.Property(d => d.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pending");
                entity.Property(d => d.Module).IsRequired().HasMaxLength(50).HasDefaultValue("Ministry");
                entity.Property(d => d.DonatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Optional FK — logged-in users get linked, guests don't
                // SetNull = donation record survives even if user account is deleted
                entity.HasOne(d => d.User)
                      .WithMany()
                      .HasForeignKey(d => d.AppUserId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });

            // ─── SEED ROLES ──────────────────────────────────────────────────
            // These 3 roles are baked in from day 1
            // Admin = full dashboard access
            // Member = ministry access (logged in)
            // YouthMember = unlocks /api/youth/* routes
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "role-admin-001",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "1"
                },
                new IdentityRole
                {
                    Id = "role-member-002",
                    Name = "Member",
                    NormalizedName = "MEMBER",
                    ConcurrencyStamp = "2"
                },
                new IdentityRole
                {
                    Id = "role-youth-003",
                    Name = "YouthMember",
                    NormalizedName = "YOUTHMEMBER",
                    ConcurrencyStamp = "3"
                }
            );
        }
    }
}