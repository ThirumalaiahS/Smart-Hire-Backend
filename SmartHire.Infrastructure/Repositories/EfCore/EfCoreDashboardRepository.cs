using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Data;

namespace SmartHire.Infrastructure.Repositories.EfCore
{
    public class EfCoreDashboardRepository : EfCoreRepository, IDashboardRepository
    {
        public EfCoreDashboardRepository(IConfiguration configuration, AppDbContext db) 
            : base(configuration, db)
        {
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(string userId)
        {
            var apps = await _db.JobApplications.Where(a => a.UserId == userId).ToListAsync();
            
            int total = apps.Count;
            int interviews = apps.Count(a => a.Status == ApplicationStatus.Interview);
            int offers = apps.Count(a => a.Status == ApplicationStatus.Offer);
            int rejected = apps.Count(a => a.Status == ApplicationStatus.Rejected);
            
            int active = apps.Count(a => a.Status == ApplicationStatus.Rejected || 
                                        a.Status == ApplicationStatus.Withdrawn || 
                                        a.Status == ApplicationStatus.Ghosted);

            double responseRate = total == 0 ? 0 : (double)active / total * 100;

            return new DashboardStatsDto(total, active, interviews, offers, rejected, Math.Round(responseRate, 1));
        }

        public async Task<IEnumerable<MonthlyApplicationDto>> GetMonthlyApplicationsAsync(string userId)
        {
            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
            var apps = await _db.JobApplications
                .Where(a => a.UserId == userId && a.AppliedDate >= sixMonthsAgo)
                .ToListAsync();

            return apps.GroupBy(a => new { a.AppliedDate.Year, a.AppliedDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlyApplicationDto(
                    new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    g.Count(),
                    g.Count(a => a.Status == ApplicationStatus.Interview)
                ));
        }
    }
}
