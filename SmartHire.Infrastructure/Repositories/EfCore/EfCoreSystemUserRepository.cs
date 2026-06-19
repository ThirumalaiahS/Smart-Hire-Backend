using Microsoft.Extensions.Configuration;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Infrastructure.Repositories.EfCore
{
    public class EfCoreSystemUserRepository : EfCoreRepository, ISystemUserRepository
    {
        public EfCoreSystemUserRepository(IConfiguration configuration, AppDbContext db) : base(configuration, db)
        {
        }

        public Task<bool> CreateUser(SystemUser systemUser, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
