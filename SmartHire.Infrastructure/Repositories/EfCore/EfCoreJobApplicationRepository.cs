using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Data;

namespace SmartHire.Infrastructure.Repositories.EfCore
{
    public class EfCoreJobApplicationRepository : EfCoreRepository, IJobApplicationRepository
    {
        public EfCoreJobApplicationRepository(IConfiguration configuration, AppDbContext db) 
            : base(configuration, db)
        {
        }

        public async Task<JobApplication> AddAsync(JobApplication application, CancellationToken cancellationToken = default)
        {
            await _db.JobApplications.AddAsync(application, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return application;
        }

        public async Task DeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            var application = await _db.JobApplications.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, cancellationToken);
            if (application is not null)
            {
                _db.JobApplications.Remove(application);
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<JobApplication>> GetAllByUserIdAsync(string userId, ApplicationStatus? status = null, CancellationToken cancellationToken = default)
        {
            var query = _db.JobApplications.Where(a => a.UserId == userId);
            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }
            return await query.OrderByDescending(a => a.AppliedDate).ToListAsync(cancellationToken);
        }

        public async Task<JobApplication?> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            return await _db.JobApplications.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, cancellationToken);
        }

        public async Task UpdateAsync(JobApplication application, CancellationToken cancellationToken = default)
        {
            application.UpdatedAt = DateTime.UtcNow;
            _db.JobApplications.Update(application);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
