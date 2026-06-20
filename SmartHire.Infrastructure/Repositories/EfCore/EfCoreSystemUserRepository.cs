using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Infrastructure.Repositories.EfCore
{
    public class EfCoreSystemUserRepository : EfCoreRepository, ISystemUserRepository
    {
        public EfCoreSystemUserRepository(IConfiguration configuration, AppDbContext db) : base(configuration, db)
        {
        }

        public async Task<bool> CreateUser(SystemUser systemUser, CancellationToken cancellationToken)
        {
            await _db.SystemUsers.AddAsync(systemUser, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteUser(string id, CancellationToken cancellationToken)
        {
            var sysUser = await _db.SystemUsers.FirstOrDefaultAsync(x => x.IdentityUserId == id, cancellationToken);
            if (sysUser == null)
                return false;

            _db.SystemUsers.Remove(sysUser);
            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> UpdateUserStatus(string id, bool isActive, CancellationToken cancellationToken)
        {
            var sysUser = await _db.SystemUsers.FirstOrDefaultAsync(x => x.IdentityUserId == id, cancellationToken);
            if (sysUser == null)
                return false;

            sysUser.IsActive = isActive;
            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
