using Microsoft.Extensions.Configuration;
using static SmartHire.Core.Common.CommonEnums;

namespace SmartHire.Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly string _connectionString;
        protected readonly DataProviderType _dataProviderType;

        protected BaseRepository(IConfiguration configuration, DataProviderType dataProviderType)
        {
            _dataProviderType = dataProviderType;
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public DataProviderType GetDataProviderType() => _dataProviderType;
    }
}
