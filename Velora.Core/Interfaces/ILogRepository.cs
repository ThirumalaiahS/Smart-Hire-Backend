using Velora.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velora.Core.Interfaces
{
    public interface ILogRepository
    {
        Task CreateErrorLogAsync(ErrorLogs errorLogs, CancellationToken cancellationToken = default);
    }
}

