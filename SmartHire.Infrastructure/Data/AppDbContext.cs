using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartHire.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Infrastructure.Data
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<JobApplication>(e =>
            {
                e.HasKey(x => x.Id);
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

            // DataProvider master
            builder.Entity<DataProvider>(e =>
            {
                e.HasKey(x => x.Id);
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
        }
    }
}
