using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Data;
using System.Security.Claims;
using static SmartHire.Core.Common.CommonEnums;

namespace SmartHire.Infrastructure.Services
{
    public class DataProviderService : IDataProviderService
    {
        private readonly AppDbContext _db;
        private readonly IUserContextService _userContextService;

        public DataProviderService(AppDbContext db, IUserContextService userContextService)
        {
            _db = db;
            _userContextService = userContextService;
        }

        public async Task<DataProviderType> GetCurrentUserDataProviderTypeAsync(CancellationToken cancellationToken = default)
        {
            var userId = _userContextService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return DataProviderType.EFCore; // Default
            }

            var settings = await _db.UserSettings
                .Include(us => us.DataProvider)
                .FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken);

            if (settings == null)
            {
                return DataProviderType.EFCore;
            }

            return settings.DataProvider.Name switch
            {
                "EFCore" => DataProviderType.EFCore,
                "Dapper" => DataProviderType.Dapper,
                "AdoNet" => DataProviderType.AdoNet,
                _ => DataProviderType.EFCore
            };
        }
    }
}
