using Microsoft.Extensions.Configuration;
using static Velora.Core.Common.CommonEnums;

namespace Velora.Infrastructure.Repositories.Dapper
{
    public abstract class DapperRepository : BaseRepository
    {
        protected DapperRepository(IConfiguration configuration) 
            : base(configuration, DataProviderType.Dapper)
        {
        }
    }
}

