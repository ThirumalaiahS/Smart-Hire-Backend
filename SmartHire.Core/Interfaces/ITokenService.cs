using SmartHire.Core.Entities;

namespace SmartHire.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
