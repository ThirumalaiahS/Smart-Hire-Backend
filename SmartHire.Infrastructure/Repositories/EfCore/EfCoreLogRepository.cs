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

        public async Task CreateErrorLogAsync(ErrorLogs errorLogs, CancellationToken cancellationToken = default)
        {
            await _db.ErrorLogs.AddAsync(errorLogs, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
