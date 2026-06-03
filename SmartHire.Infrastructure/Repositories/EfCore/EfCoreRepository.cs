using Microsoft.Extensions.Configuration;
using SmartHire.Infrastructure.Data;
using static SmartHire.Core.Common.CommonEnums;

namespace SmartHire.Infrastructure.Repositories.EfCore
{
    public abstract class EfCoreRepository : BaseRepository
    {
        protected readonly AppDbContext _db;

        protected EfCoreRepository(IConfiguration configuration, AppDbContext db) 
            : base(configuration, DataProviderType.EFCore)
        {
            _db = db;
        }
    }
}
