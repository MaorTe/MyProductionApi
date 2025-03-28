using Microsoft.EntityFrameworkCore;
using MyProductionApi.Data;
using MyProductionApi.Models;
using System.Text.Json;

namespace MyProductionApi.Services;

public class EfCoreCache : ICacheService
{
    private readonly AppDbContext _db;

    public EfCoreCache(AppDbContext db) {
        _db = db;
    }

    public async Task SetAsync<T>(string key, T value, int ttl) {
        string seializedData = JsonSerializer.Serialize(value);
        
        CacheEntry c = new() {
            Key = key,
            SerializedData = seializedData,
            Timestmap = DateTime.UtcNow,
            TTL = ttl
        };
        _db.CacheEntries.Add(c);
        await _db.SaveChangesAsync();
    }

    public async Task<T?> GetAsync<T>(string key) {
        CacheEntry? cachedItem = await _db.CacheEntries.FirstOrDefaultAsync(c => c.Key == key);
        if (cachedItem is null) {
            return default;
        }

        DateTime expiration = cachedItem.Timestmap.AddSeconds(cachedItem.TTL);
        bool isExpired = DateTime.UtcNow > expiration;
        if (isExpired) {
            _db.CacheEntries.Remove(cachedItem);
            await _db.SaveChangesAsync();
            return default;
        }

        return JsonSerializer.Deserialize<T>(cachedItem.SerializedData);
    }
}
