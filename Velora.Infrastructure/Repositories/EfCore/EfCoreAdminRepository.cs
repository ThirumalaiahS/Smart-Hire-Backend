using Microsoft.Extensions.Configuration;
using Velora.Core.Interfaces;
using Velora.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velora.Infrastructure.Repositories.EfCore
{
    public class EfCoreAdminRepository : EfCoreRepository, IAdminRepository
    {
        public EfCoreAdminRepository(IConfiguration configuration, AppDbContext db) : base(configuration, db)
        {
        }
    }
}

