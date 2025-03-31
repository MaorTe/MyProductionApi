using Microsoft.EntityFrameworkCore;
using MyProductionApi.Models;

namespace MyProductionApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CacheEntry> CacheEntries { get; set; }
}
