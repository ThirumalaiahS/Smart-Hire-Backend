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
    public class EfCoreLogRepository : EfCoreRepository, ILogRepository
    {
        public EfCoreLogRepository(IConfiguration configuration, AppDbContext db)
            : base(configuration, db)
        {
        }

        public async Task CreateErrorLogAsync(ErrorLogs errorLogs)
        {
            await _db.ErrorLogs.AddAsync(errorLogs);
            await _db.SaveChangesAsync();
        }
    }
}
