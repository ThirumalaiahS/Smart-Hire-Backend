using static SmartHire.Core.Common.CommonEnums;

namespace SmartHire.Core.Interfaces
{
    public interface IDataProviderService
    {
        Task<DataProviderType> GetCurrentUserDataProviderTypeAsync(CancellationToken cancellationToken = default);
    }
}
