namespace MyProductionApi.Services;

public interface ICacheService
{
    Task SetAsync<T>(string key, T value, int ttl);
    
    Task<T?> GetAsync<T>(string key);
}
    
