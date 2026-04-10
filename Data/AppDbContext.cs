using GlobalFlameMinistry.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        // ── DB SETS ────────────────────────────────────────────────────────────
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
        public DbSet<PrayerRequest> PrayerRequests { get; set; }
        public DbSet<Testimony> Testimonies { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Sermon> Sermons { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BulkEmailMessage> BulkEmailMessages { get; set; }
        public DbSet<MinistryDepartment> MinistryDepartments { get; set; }
        public DbSet<CounsellingRequest> CounsellingRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ── APP USER ───────────────────────────────────────────────────────
            builder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.ProfilePictureUrl).HasMaxLength(500);
                entity.Property(u => u.RefreshToken).HasMaxLength(500);
                entity.Property(u => u.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            });

            // ── ANNOUNCEMENT ───────────────────────────────────────────────────
            builder.Entity<Announcement>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Title).IsRequired().HasMaxLength(200);
                entity.Property(a => a.Content).IsRequired();
                entity.Property(a => a.CreatedById).IsRequired();
                entity.Property(a => a.Module).IsRequired().HasMaxLength(50).HasDefaultValue("Ministry");
                entity.Property(a => a.Category).HasMaxLength(100);
                entity.Property(a => a.IsPublished).HasDefaultValue(false);
                entity.Property(a => a.IsDeleted).HasDefaultValue(false);
                entity.Property(a => a.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
                entity.HasQueryFilter(a => !a.IsDeleted);
            });

            // ── EVENT ──────────────────────────────────────────────────────────
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
                entity.HasOne(e => e.Ministry)
                      .WithMany(m => m.Events)
                      .HasForeignKey(e => e.MinistryId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });

            // ── EVENT REGISTRATION ─────────────────────────────────────────────
            builder.Entity<EventRegistration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.AppUserId).HasMaxLength(450).IsRequired(false);

                entity.HasOne(e => e.Event)
                      .WithMany()
                      .HasForeignKey(e => e.EventId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.AppUserId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });

            // ── PRAYER REQUEST ─────────────────────────────────────────────────
            builder.Entity<PrayerRequest>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Email).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Topic).HasMaxLength(50);
                entity.Property(p => p.Content).IsRequired().HasMaxLength(1000);
                entity.Property(p => p.PhoneNumber).HasMaxLength(20);
                entity.Property(p => p.PreferredContact).HasMaxLength(20).HasDefaultValue("Email");
                entity.Property(p => p.Attachment).HasMaxLength(500);
                entity.Property(p => p.IsAttendedTo).HasDefaultValue(false);
                entity.Property(p => p.IsDeleted).HasDefaultValue(false);
                entity.Property(p => p.AnonymousToken).IsRequired().HasMaxLength(100);
                entity.HasIndex(p => p.AnonymousToken).IsUnique();
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasQueryFilter(p => !p.IsDeleted);

                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.AppUserId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });

            // ── TESTIMONY ──────────────────────────────────────────────────────
            builder.Entity<Testimony>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.FullName).HasMaxLength(150);
                entity.Property(t => t.Content).IsRequired().HasMaxLength(2000);
                entity.Property(t => t.Attachment).HasMaxLength(500);
                entity.Property(t => t.Status)
                      .HasDefaultValue(TestimonyStatus.Pending)
                      .HasConversion<int>();
                entity.Property(t => t.IsDeleted).HasDefaultValue(false);
                entity.Property(t => t.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Testimonies)
                      .HasForeignKey(t => t.AppUserId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
                entity.HasQueryFilter(t => !t.IsDeleted);
            });

            // ── CONTACT ────────────────────────────────────────────────────────
            builder.Entity<Contact>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.FullName).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
                entity.Property(c => c.PhoneNumber).HasMaxLength(20);
                entity.Property(c => c.Message).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(c => c.Status).IsRequired().HasDefaultValue(ContactMessageStatus.New);
                entity.Property(c => c.Type).IsRequired().HasDefaultValue(ContactMessageType.General);
                entity.Property(c => c.IsDeleted).HasDefaultValue(false);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(c => c.CreatedAt);
                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            // ── DONATION ───────────────────────────────────────────────────────
            builder.Entity<Donation>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.DonorName).IsRequired().HasMaxLength(150);
                entity.Property(d => d.DonorEmail).IsRequired().HasMaxLength(200);
                entity.Property(d => d.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(d => d.Currency).HasMaxLength(10).HasDefaultValue("NGN");
                entity.Property(d => d.TransactionReference).IsRequired().HasMaxLength(200);
                entity.HasIndex(d => d.TransactionReference).IsUnique();
                entity.Property(d => d.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pending");
                entity.Property(d => d.SubaccountCode).HasMaxLength(100);
                entity.Property(d => d.EventTitle).HasMaxLength(300);
                entity.Property(d => d.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(d => d.AppUserId).HasMaxLength(450).IsRequired(false);

                entity.HasOne(d => d.User)
                      .WithMany()
                      .HasForeignKey(d => d.AppUserId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });

            // ── BOOK ───────────────────────────────────────────────────────────
            builder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(150);
                entity.Property(b => b.Description).HasMaxLength(5000);
                entity.Property(b => b.CoverImageUrl).HasMaxLength(500);
                entity.Property(b => b.AmazonUrl).HasMaxLength(500);
                entity.Property(b => b.SelarUrl).HasMaxLength(500);
                entity.Property(b => b.Price).HasColumnType("decimal(18,2)");
                entity.Property(b => b.Currency).HasMaxLength(10).HasDefaultValue("NGN");
                entity.Property(b => b.IsFeatured).HasDefaultValue(false);
                entity.Property(b => b.IsPublished).HasDefaultValue(false);
                entity.Property(b => b.IsDeleted).HasDefaultValue(false);
                entity.Property(b => b.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
                entity.HasQueryFilter(b => !b.IsDeleted);
            });

            // ── BULK EMAIL ─────────────────────────────────────────────────────
            builder.Entity<BulkEmailMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(300);
                entity.Property(e => e.HtmlBody).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(e => e.TargetGroup).HasMaxLength(20).HasDefaultValue("All");
                entity.Property(e => e.CustomEmailsJson).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Scheduled");
                entity.Property(e => e.TotalRecipients).HasDefaultValue(0);
                entity.Property(e => e.SuccessCount).HasDefaultValue(0);
                entity.Property(e => e.FailedCount).HasDefaultValue(0);
                entity.Property(e => e.CreatedByUserId).HasMaxLength(450);
                entity.Property(e => e.CreatedByName).HasMaxLength(200);
                entity.Property(e => e.ErrorMessage).HasColumnType("nvarchar(max)");
                entity.Property(e => e.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            });

            // ── MINISTRY DEPARTMENT ────────────────────────────────────────────
            builder.Entity<MinistryDepartment>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Name).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Slug).IsRequired().HasMaxLength(200);
                entity.Property(m => m.ShortDescription).IsRequired().HasMaxLength(500);
                entity.Property(m => m.Description).HasColumnType("nvarchar(max)");
                entity.Property(m => m.CoverImageUrl).HasMaxLength(500);
                entity.Property(m => m.LeaderName).HasMaxLength(200);
                entity.Property(m => m.LeaderTitle).HasMaxLength(200);
                entity.Property(m => m.LeaderImageUrl).HasMaxLength(500);
                entity.Property(m => m.ContactEmail).HasMaxLength(200);
                entity.Property(m => m.DisplayOrder).HasDefaultValue(0);
                entity.Property(m => m.IsPublished).HasDefaultValue(false);
                entity.Property(m => m.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(m => m.Slug).IsUnique();
            });

            // ── COUNSELLING REQUEST ────────────────────────────────────────────
            builder.Entity<CounsellingRequest>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.FullName).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
                entity.Property(c => c.PhoneNumber).HasMaxLength(20);
                entity.Property(c => c.Topic).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Message).IsRequired().HasColumnType("nvarchar(max)");
                entity.Property(c => c.PreferredContact).HasMaxLength(20).HasDefaultValue("Email");
                entity.Property(c => c.AssignedTo).HasMaxLength(200);
                entity.Property(c => c.AssignedToEmail).HasMaxLength(200);
                entity.Property(c => c.Status)
                      .HasDefaultValue(CounsellingStatus.New)
                      .HasConversion<int>();
                entity.Property(c => c.AppUserId).HasMaxLength(450).IsRequired(false);
                entity.Property(c => c.IsDeleted).HasDefaultValue(false);
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasQueryFilter(c => !c.IsDeleted);

                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.AppUserId)
                      .OnDelete(DeleteBehavior.SetNull)
                      .IsRequired(false);
            });

            // ── SEED ROLES ─────────────────────────────────────────────────────
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