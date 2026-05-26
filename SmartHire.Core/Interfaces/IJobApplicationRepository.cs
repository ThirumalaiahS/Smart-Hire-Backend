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
        Task<JobApplication?> GetByIdAsync(int id, string userId);
        Task<IEnumerable<JobApplication>> GetAllByUserIdAsync(string userId, ApplicationStatus? status = null);
        Task<JobApplication> AddAsync(JobApplication application);
        Task UpdateAsync(JobApplication application);
        Task DeleteAsync(int id, string userId);
    }
}
