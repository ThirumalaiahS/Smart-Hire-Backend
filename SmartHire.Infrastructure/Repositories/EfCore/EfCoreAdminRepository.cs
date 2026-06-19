using Microsoft.Extensions.Configuration;
using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Infrastructure.Repositories.EfCore
{
    public class EfCoreAdminRepository : EfCoreRepository, IAdminRepository
    {
        public EfCoreAdminRepository(IConfiguration configuration, AppDbContext db) : base(configuration, db)
        {
        }
    }
}
