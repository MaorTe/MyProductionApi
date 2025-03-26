using Microsoft.EntityFrameworkCore;
using MyProductionApi.Data;
using MyProductionApi.Models;
using System.Text.Json;

namespace MyProductionApi.Services;

public class EfCoreCache : ICacheService
{
    private readonly CacheDbContext m_CacheDb;

    public EfCoreCache(CacheDbContext db) {
        m_CacheDb = db;
    }

    public async Task SetAsync<T>(string key, T value, int ttl) {
        string seializedData = JsonSerializer.Serialize(value);
        CacheEntry? existing = await m_CacheDb.CacheEntries.FirstOrDefaultAsync(c => c.Key == key);
        if (existing is not null) {
            m_CacheDb.CacheEntries.Remove(existing);
        }

        CacheEntry c = new() {
            Key = key,
            SerializedData = seializedData,
            Timestmap = DateTime.UtcNow,
            TTL = ttl
        };

        m_CacheDb.CacheEntries.Add(c);
        await m_CacheDb.SaveChangesAsync();
        // value. = DateTime.UtcNow.Add(ttl);
        //CacheEntry c = new() {
        //    Key = key,
        //    Value = System.Text.Json.JsonSerializer.Serialize(value),
        //    Expiration = DateTime.UtcNow.Add(ttl)
        //};
        //    _db.CacheEntries.Add(c);

        //await _db.SaveChangesAsync();
    }

    public async Task<T?> GetAsync<T>(string key) {
        CacheEntry? cachedItem = await m_CacheDb.CacheEntries.FirstOrDefaultAsync(c => c.Key == key);
        if (cachedItem is null) {
            return default;
        }

        DateTime expiration = cachedItem.Timestmap.AddSeconds(cachedItem.TTL);
        bool isExpired = DateTime.UtcNow > expiration;
        if (isExpired) {
            m_CacheDb.CacheEntries.Remove(cachedItem);
            await m_CacheDb.SaveChangesAsync();
            return default;
        }

        return JsonSerializer.Deserialize<T>(cachedItem.SerializedData);

        //var entry = await _db.CacheEntries.FindAsync(key);

        //if (entry == null || entry.IsExpired) {
        //    if (entry != null) {
        //        _db.CacheEntries.Remove(entry);
        //        await _db.SaveChangesAsync();
        //    }
        //    return null;
        //}

        //return entry.Value;
    }
}
