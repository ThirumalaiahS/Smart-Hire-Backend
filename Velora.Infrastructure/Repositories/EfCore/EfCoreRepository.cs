using Microsoft.Extensions.Configuration;
using Velora.Infrastructure.Data;
using static Velora.Core.Common.CommonEnums;

namespace Velora.Infrastructure.Repositories.EfCore
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

