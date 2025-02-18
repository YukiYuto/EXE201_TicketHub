namespace TicketHub.Services.IService;

public interface IRedisService
{
    Task<bool> StoreString(string key, string value, TimeSpan? expiry = null);
    Task<string> RetrieveString(string key);
    Task<bool> DeleteString(string key);
}