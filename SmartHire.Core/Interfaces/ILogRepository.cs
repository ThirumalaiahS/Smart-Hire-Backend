using SmartHire.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.Interfaces
{
    public interface ILogRepository
    {
        Task CreateErrorLogAsync(ErrorLogs errorLogs, CancellationToken cancellationToken = default);
    }
}
