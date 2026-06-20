using Microsoft.Extensions.Configuration;
using SmartHire.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Infrastructure.Repositories.AdoNet
{
    public class AdoNetAdminRepository : AdoDotNetRepository, IAdminRepository
    {
        public AdoNetAdminRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
