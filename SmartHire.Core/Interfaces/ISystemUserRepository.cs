using SmartHire.Core.DTOs.User;
using SmartHire.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.Interfaces
{
    public interface ISystemUserRepository
    {
        Task<bool> CreateUser(SystemUser systemUser, CancellationToken cancellationToken);
        Task<bool> UpdateUserStatus(string id, bool isActive, CancellationToken cancellationToken);
        Task<bool> DeleteUser(string id, CancellationToken cancellationToken);
    }
}
