using Velora.Core.DTOs;
using Velora.Core.Entities;

namespace Velora.Core.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user, CancellationToken cancellationToken = default);
        Task<UserDto> GenerateTokens(AppUser user, string ipAddress, CancellationToken cancellationToken = default);
        Task<UserDto> RefreshToken(string token, string refreshToken, string ipAddress, CancellationToken cancellationToken = default);
        Task<bool> RevokeToken(string refreshToken, string ipAddress, CancellationToken cancellationToken = default);
    }
}

