using SmartHire.Core.Entities;

namespace SmartHire.Core.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
