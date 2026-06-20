using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;

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

        private async Task<IJobApplicationRepository> GetRepositoryAsync(CancellationToken cancellationToken)
        {
            var providerType = await _dataProviderService.GetCurrentUserDataProviderTypeAsync(cancellationToken);
            
            // The specific repositories inherit from BaseRepository which has GetDataProviderType()
            // We need to find the one that matches.
            var repo = _repositories.FirstOrDefault(r => 
                r is BaseRepository baseRepo && baseRepo.GetDataProviderType() == providerType);

            return repo ?? _repositories.First(r => r is not JobApplicationRepository);
        }

        public async Task<JobApplication> AddAsync(JobApplication application, CancellationToken cancellationToken = default)
        {
            var repo = await GetRepositoryAsync(cancellationToken);
            return await repo.AddAsync(application, cancellationToken);
        }

        public async Task DeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            var repo = await GetRepositoryAsync(cancellationToken);
            await repo.DeleteAsync(id, userId, cancellationToken);
        }

        public async Task<IEnumerable<JobApplication>> GetAllByUserIdAsync(string userId, ApplicationStatus? status = null, CancellationToken cancellationToken = default)
        {
            var repo = await GetRepositoryAsync(cancellationToken);
            return await repo.GetAllByUserIdAsync(userId, status, cancellationToken);
        }

        public async Task<JobApplication?> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default)
        {
            var repo = await GetRepositoryAsync(cancellationToken);
            return await repo.GetByIdAsync(id, userId, cancellationToken);
        }

        public async Task UpdateAsync(JobApplication application, CancellationToken cancellationToken = default)
        {
            var repo = await GetRepositoryAsync(cancellationToken);
            await repo.UpdateAsync(application, cancellationToken);
        }
    }
}
