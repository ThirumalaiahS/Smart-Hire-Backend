using Microsoft.EntityFrameworkCore;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Data;

namespace SmartHire.Infrastructure.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly AppDbContext _db;

        public JobApplicationRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<JobApplication> AddAsync(JobApplication application)
        {
            await _db.JobApplications.AddAsync(application);
            await _db.SaveChangesAsync();
            return application;
        }

        public async Task DeleteAsync(int id, string userId)
        {
           var application = await _db.JobApplications.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (application is not null)
            {
                _db.JobApplications.Remove(application);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<JobApplication>> GetAllByUserIdAsync(string userId, ApplicationStatus? status = null)
        {
            var query = _db.JobApplications.Where(a => a.UserId == userId);
            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }
            return await query.OrderByDescending(a => a.AppliedDate).ToListAsync();
        }

        public async Task<JobApplication?> GetByIdAsync(int id, string userId)
        {
            return await _db.JobApplications.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        }

        public async Task UpdateAsync(JobApplication application)
        {
            application.UpdatedAt = DateTime.UtcNow;
            _db.JobApplications.Update(application);
            await _db.SaveChangesAsync();
        }
    }
}
