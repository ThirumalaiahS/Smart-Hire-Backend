using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Repositories.AdoNet;
using SmartHire.Infrastructure.Repositories.Dapper;
using SmartHire.Infrastructure.Repositories.EfCore;
using static SmartHire.Core.Common.CommonEnums;

namespace SmartHire.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IEnumerable<IDashboardRepository> _repositories;
        private readonly IDataProviderService _dataProviderService;

        public DashboardRepository(
            IEnumerable<IDashboardRepository> repositories,
            IDataProviderService dataProviderService)
        {
            _repositories = repositories;
            _dataProviderService = dataProviderService;
        }

        private async Task<IDashboardRepository> GetRepositoryAsync()
        {
            var providerType = await _dataProviderService.GetCurrentUserDataProviderTypeAsync();
            
            var repo = _repositories.FirstOrDefault(r => 
                r is BaseRepository baseRepo && baseRepo.GetDataProviderType() == providerType);

            return repo ?? _repositories.First(r => r is not DashboardRepository);
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(string userId)
        {
            var repo = await GetRepositoryAsync();
            return await repo.GetDashboardStatsAsync(userId);
        }

        public async Task<IEnumerable<MonthlyApplicationDto>> GetMonthlyApplicationsAsync(string userId)
        {
            var repo = await GetRepositoryAsync();
            return await repo.GetMonthlyApplicationsAsync(userId);
        }
    }
}
