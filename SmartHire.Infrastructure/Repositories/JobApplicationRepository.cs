using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Repositories.AdoNet;
using SmartHire.Infrastructure.Repositories.Dapper;
using SmartHire.Infrastructure.Repositories.EfCore;
using static SmartHire.Core.Common.CommonEnums;

namespace SmartHire.Infrastructure.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly IEnumerable<IJobApplicationRepository> _repositories;
        private readonly IDataProviderService _dataProviderService;

        public JobApplicationRepository(
            IEnumerable<IJobApplicationRepository> repositories,
            IDataProviderService dataProviderService)
        {
            _repositories = repositories;
            _dataProviderService = dataProviderService;
        }

        private async Task<IJobApplicationRepository> GetRepositoryAsync()
        {
            var providerType = await _dataProviderService.GetCurrentUserDataProviderTypeAsync();
            
            // The specific repositories inherit from BaseRepository which has GetDataProviderType()
            // We need to find the one that matches.
            var repo = _repositories.FirstOrDefault(r => 
                r is BaseRepository baseRepo && baseRepo.GetDataProviderType() == providerType);

            return repo ?? _repositories.First(r => r is not JobApplicationRepository);
        }

        public async Task<JobApplication> AddAsync(JobApplication application)
        {
            var repo = await GetRepositoryAsync();
            return await repo.AddAsync(application);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var repo = await GetRepositoryAsync();
            await repo.DeleteAsync(id, userId);
        }

        public async Task<IEnumerable<JobApplication>> GetAllByUserIdAsync(string userId, ApplicationStatus? status = null)
        {
            var repo = await GetRepositoryAsync();
            return await repo.GetAllByUserIdAsync(userId, status);
        }

        public async Task<JobApplication?> GetByIdAsync(int id, string userId)
        {
            var repo = await GetRepositoryAsync();
            return await repo.GetByIdAsync(id, userId);
        }

        public async Task UpdateAsync(JobApplication application)
        {
            var repo = await GetRepositoryAsync();
            await repo.UpdateAsync(application);
        }
    }
}
