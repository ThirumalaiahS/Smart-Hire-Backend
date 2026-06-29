using static Velora.Core.Common.CommonEnums;

namespace Velora.Core.Interfaces
{
    public interface IDataProviderService
    {
        Task<DataProviderType> GetCurrentUserDataProviderTypeAsync(CancellationToken cancellationToken = default);
    }
}

