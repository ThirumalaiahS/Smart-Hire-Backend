using Microsoft.Extensions.Configuration;
using SmartHire.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Infrastructure.Repositories.Dapper
{
    public class DapperAdminRepository : DapperRepository, IAdminRepository
    {
        public DapperAdminRepository(IConfiguration configuration) 
            : base(configuration)
        {
        }
    }
}
