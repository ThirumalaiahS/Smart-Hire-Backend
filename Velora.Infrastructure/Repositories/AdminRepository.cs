using Velora.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velora.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IEnumerable<IAdminRepository> _adminRepositories;
        private readonly IDataProviderService _dataProviderService;

        public AdminRepository(IEnumerable<IAdminRepository> adminRepositories, IDataProviderService dataProviderService)
        {
            _adminRepositories = adminRepositories;
            _dataProviderService = dataProviderService;
        }
    }
}

