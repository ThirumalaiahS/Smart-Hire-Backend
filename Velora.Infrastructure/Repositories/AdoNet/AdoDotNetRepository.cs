using Microsoft.Extensions.Configuration;
using static Velora.Core.Common.CommonEnums;

namespace Velora.Infrastructure.Repositories.AdoNet
{
    public abstract class AdoDotNetRepository : BaseRepository
    {
        protected AdoDotNetRepository(IConfiguration configuration) 
            : base(configuration, DataProviderType.AdoNet)
        {
        }
    }
}

