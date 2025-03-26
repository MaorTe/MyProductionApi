using Microsoft.EntityFrameworkCore;
using MyProductionApi.Models;

namespace MyProductionApi.Data;

public class CacheDbContext : DbContext
{
    public DbSet<CacheEntry> CacheEntries { get; set; }

    public CacheDbContext(DbContextOptions<CacheDbContext> options) : base(options) { }
}
