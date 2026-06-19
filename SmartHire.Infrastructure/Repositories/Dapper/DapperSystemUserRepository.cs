using Microsoft.Extensions.Configuration;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Infrastructure.Repositories.Dapper
{
    public class DapperSystemUserRepository : DapperRepository, ISystemUserRepository
    {
        public DapperSystemUserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Task<bool> CreateUser(SystemUser systemUser, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
