using Microsoft.Extensions.Configuration;
using static SmartHire.Core.Common.CommonEnums;

namespace SmartHire.Infrastructure.Repositories.Dapper
{
    public abstract class DapperRepository : BaseRepository
    {
        protected DapperRepository(IConfiguration configuration) 
            : base(configuration, DataProviderType.Dapper)
        {
        }
    }
}
