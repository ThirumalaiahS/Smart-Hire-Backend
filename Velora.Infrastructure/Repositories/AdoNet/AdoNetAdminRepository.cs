using Microsoft.Extensions.Configuration;
using Velora.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velora.Infrastructure.Repositories.AdoNet
{
    public class AdoNetAdminRepository : AdoDotNetRepository, IAdminRepository
    {
        public AdoNetAdminRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}

