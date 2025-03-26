using System.ComponentModel.DataAnnotations;

namespace MyProductionApi.Models;

public class CacheEntry
{
    //[Key]
    //public string Key { get; set; } = string.Empty;

    //public string Value { get; set; } = string.Empty;

    //public DateTime Expiration { get; set; }

    //public bool IsExpired => DateTime.UtcNow > Expiration;

    [Key]
    public string? Key { get; set; }
    public string? SerializedData { get; set; }
    public DateTime Timestmap { get; set; }
    public int TTL { get; set; }
}