using g_flame_youth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace g_flame_youth.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<PrayerRequest> PrayerRequests { get; set; }
        public DbSet<Devotional> Devotionals { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
           .HasIndex(u => u.Email)
           .IsUnique();

            builder.Entity<Announcement>().Property(a => a.CreatedById).IsRequired();

            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.StartDate).IsRequired();

                entity.Property(e => e.EndDate).IsRequired();

                entity.Property(e => e.Location).IsRequired().HasMaxLength(300);

                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                entity.Property(e => e.IsCancelled).HasDefaultValue(false);

                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            });

            builder.Entity<PrayerRequest>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
            });

            builder.Entity<Devotional>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Title).IsRequired().HasMaxLength(200);
                entity.Property(d => d.Content).IsRequired().HasMaxLength(2000);
                entity.Property(d => d.DevotionalDate).IsRequired();
                entity.Property(d => d.IsPublished).HasDefaultValue(false);
                entity.Property(d => d.IsDeleted).HasDefaultValue(false);
                entity.HasIndex(d => d.DevotionalDate).IsUnique();
                entity.HasQueryFilter(d => !d.IsDeleted);
                entity.Property(d => d.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}