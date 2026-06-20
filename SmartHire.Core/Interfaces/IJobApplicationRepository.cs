using SmartHire.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication?> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<JobApplication>> GetAllByUserIdAsync(string userId, ApplicationStatus? status = null, CancellationToken cancellationToken = default);
        Task<JobApplication> AddAsync(JobApplication application, CancellationToken cancellationToken = default);
        Task UpdateAsync(JobApplication application, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, string userId, CancellationToken cancellationToken = default);
    }
}
