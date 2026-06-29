using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Velora.Core.Entities;

namespace Velora.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<DataProvider> DataProviders => Set<DataProvider>();
        public DbSet<UserSettings> UserSettings => Set<UserSettings>();
        public DbSet<ErrorLogs> ErrorLogs => Set<ErrorLogs>();
        public DbSet<SystemUser> SystemUsers => Set<SystemUser>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<JobApplication>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasIndex(x => x.UserId);
                e.HasIndex(x => new { x.UserId, x.Status });
                e.Property(x => x.CompanyName).IsRequired().HasMaxLength(200);
                e.Property(x => x.JobTitle).IsRequired().HasMaxLength(200);
                e.Property(x => x.Status).HasConversion<string>();
                e.HasOne(x => x.User)
                .WithMany(u => u.JobApplications)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ChatMessage>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasIndex(x => x.UserId);
                e.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // DataProvider master
            builder.Entity<DataProvider>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            });

            // UserSettings
            builder.Entity<UserSettings>(entity =>
            {
                entity.HasKey(us => us.UserId);
                entity.HasOne(entity => entity.DataProvider)
                    .WithMany(dp => dp.UserSettings)
                    .HasForeignKey(us => us.DataProviderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed master providers
            builder.Entity<DataProvider>().HasData(
                new DataProvider { Id = 1, Name = "EFCore" },
                new DataProvider { Id = 2, Name = "Dapper" },
                new DataProvider { Id = 3, Name = "AdoNet" }
            );

            builder.Entity<ErrorLogs>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            builder.Entity<SystemUser>(e =>
            {
                e.HasKey(x => x.UserId);
                e.Property(x => x.UserId).ValueGeneratedOnAdd();
                e.Property(x => x.FullName).IsRequired().HasMaxLength(100);
                e.Property(x => x.Email).IsRequired().HasMaxLength(200);
                e.Property(x => x.IdentityUserId).IsRequired().HasConversion<string>();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            builder.Entity<RefreshToken>(e =>
            {
                e.HasKey(x => x.RefreshTokenId);
                e.Property(x => x.RefreshTokenId).ValueGeneratedOnAdd();
                e.Property(x => x.Token).IsRequired().HasMaxLength(1000);
                e.HasIndex(x => x.Token).IsUnique();
                e.Property(x => x.SysUserId).IsRequired();
                e.Property(x => x.IsRevoked).HasDefaultValue<bool>(false);
                e.Property(x => x.IsUsed).HasDefaultValue<bool>(false);
                e.Property(x => x.JwtId).HasMaxLength(100);
                e.Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.ExpiresOn).IsRequired();
                e.HasOne(x => x.SystemUser)
                    .WithMany(x => x.RefreshTokens)
                    .HasForeignKey(x => x.SysUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

