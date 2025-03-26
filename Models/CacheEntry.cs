using System.ComponentModel.DataAnnotations;

namespace MyProductionApi.Models;

public class CacheEntry
{
    [Key]
    public string? Key { get; set; }
    public string? SerializedData { get; set; }
    public DateTime Timestmap { get; set; }
    public int TTL { get; set; }
}