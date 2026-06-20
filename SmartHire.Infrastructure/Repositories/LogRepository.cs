using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;

namespace SmartHire.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly IEnumerable<ILogRepository> _repositories;
        private readonly IDataProviderService _dataProviderService;

        public LogRepository(
            IEnumerable<ILogRepository> repositories,
            IDataProviderService dataProviderService)
        {
            _repositories = repositories;
            _dataProviderService = dataProviderService;
        }

        private async Task<ILogRepository> GetRepositoryAsync(CancellationToken cancellationToken)
        {
            var providerType = await _dataProviderService.GetCurrentUserDataProviderTypeAsync(cancellationToken);

            var repo = _repositories.FirstOrDefault(r =>
                r is BaseRepository baseRepo && baseRepo.GetDataProviderType() == providerType);

            return repo ?? _repositories.First(r => r is not LogRepository);
        }

        public async Task CreateErrorLogAsync(ErrorLogs errorLogs, CancellationToken cancellationToken = default)
        {
            var repo = await GetRepositoryAsync(cancellationToken);
            await repo.CreateErrorLogAsync(errorLogs, cancellationToken);
        }
    }
}
