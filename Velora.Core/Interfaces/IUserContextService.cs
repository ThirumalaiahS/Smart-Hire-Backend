namespace Velora.Core.Interfaces
{
    public interface IUserContextService
    {
        string? GetUserId();
        bool IsAuthenticated();
    }
}

