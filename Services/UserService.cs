using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyProductionApi.Data;
using MyProductionApi.Models;

namespace MyProductionApi.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly ICacheService _cache;

    public UserService(AppDbContext db, ICacheService cache) 
    {
        _db = db;
        _cache = cache;
    }

    public async Task<List<User>> GetAllUsers() {

        List<User> cachedUsers = await _cache.GetAsync<List<User>>("users");
        if(cachedUsers != null) return cachedUsers;

        var users = await _db.Users.ToListAsync();
        await _cache.SetAsync("users", users, 600);
        return users;
    }
    
    public async Task<User?> GetUserById(Guid id) =>
        await _db.Users.FindAsync(id);
    
    public async Task<User> GetUserByEmail(string email) {
        throw new NotImplementedException();
    }

    public async Task<User> CreateUser(User user) {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }
}
