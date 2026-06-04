namespace SmartHire.Core.Interfaces
{
    public interface IUserContextService
    {
        string? GetUserId();
        bool IsAuthenticated();
    }
}
