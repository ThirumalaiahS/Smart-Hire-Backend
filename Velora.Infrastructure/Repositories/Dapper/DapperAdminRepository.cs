using Microsoft.Extensions.Configuration;
using Velora.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velora.Infrastructure.Repositories.Dapper
{
    public class DapperAdminRepository : DapperRepository, IAdminRepository
    {
        public DapperAdminRepository(IConfiguration configuration) 
            : base(configuration)
        {
        }
    }
}

