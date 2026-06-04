using Microsoft.Extensions.Configuration;
using static SmartHire.Core.Common.CommonEnums;

namespace SmartHire.Infrastructure.Repositories.AdoNet
{
    public abstract class AdoDotNetRepository : BaseRepository
    {
        protected AdoDotNetRepository(IConfiguration configuration) 
            : base(configuration, DataProviderType.AdoNet)
        {
        }
    }
}
