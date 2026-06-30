using Velora.Core.Entities;
using Velora.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velora.Infrastructure.Repositories
{
    public class SystemUserRepository : ISystemUserRepository
    {
        private readonly IEnumerable<ISystemUserRepository> _repositories;
        private readonly IDataProviderService _dataProviderService;

        public SystemUserRepository(IEnumerable<ISystemUserRepository> repositories, IDataProviderService dataProviderService)
        {
            _repositories = repositories;
            _dataProviderService = dataProviderService;
        }

        public async Task<ISystemUserRepository> GetRepository(CancellationToken cancellation)
        {
            var providerType = await _dataProviderService.GetCurrentUserDataProviderTypeAsync(cancellation);

            var repo = _repositories.FirstOrDefault(r =>
                r is BaseRepository baseRepo && baseRepo.GetDataProviderType() == providerType);

            return repo ?? _repositories.First(r => r is not SystemUserRepository);
        }

        public async Task<bool> CreateUser(SystemUser systemUser, CancellationToken cancellationToken)
        {
            var repo = await GetRepository(cancellationToken);
            return await repo.CreateUser(systemUser, cancellationToken);
        }

        public async Task<bool> UpdateUserStatus(string id, bool isActive, CancellationToken cancellationToken)
        {
            var repo = await GetRepository(cancellationToken);
            return await repo.UpdateUserStatus(id, isActive, cancellationToken);
        }

        public async Task<bool> DeleteUser(string id, CancellationToken cancellationToken)
        {
            var repo = await GetRepository(cancellationToken);
            return await repo.DeleteUser(id, cancellationToken);
        }
    }
}

