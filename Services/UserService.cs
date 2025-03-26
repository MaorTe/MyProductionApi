using Microsoft.EntityFrameworkCore;
using MyProductionApi.Data;
using MyProductionApi.Models;
using System.Text.Json;

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
  
    public async Task<List<User>> GetAllUsers(int pageSize) {
        List<User> cachedUsers = await _cache.GetAsync<List<User>>("users");
        if (cachedUsers != null) return cachedUsers;

        var users = await GetUsersFromApi($"https://randomuser.me/api/?results={pageSize}");
        await _cache.SetAsync("users", users, 600);
        return users;
    }
    
    public async Task<User> GetUserById(Guid id) =>
        await _db.Users.FindAsync(id);

    public async Task<List<User>> SearchUsers(string searchTerm) {
        return await _db.Users
            .Where(u =>
                u.FirstName.Contains(searchTerm) ||
                u.LastName.Contains(searchTerm) ||
                u.Email.Contains(searchTerm) ||
                (u.Phone != null && u.Phone.Contains(searchTerm)) ||
                (u.Address != null && u.Address.Contains(searchTerm))
            )
            .ToListAsync();
    }

    public async Task<User> CreateUser(User user) {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    private async Task<List<User>> GetUsersFromApi(string url) {
        using HttpClient client = new HttpClient();

        try {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            var randomUser = JsonSerializer.Deserialize<RandomApiUser>(responseBody, new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            });
            var result = randomUser?.results;
            if (result == null) return null;

            List<User> usersList = new();
            foreach (var user in result) 
            {
                usersList.Add(new User {
                    Id = Guid.NewGuid(),
                    FirstName = user.name.first,
                    LastName = user.name.last,
                    Email = user.email,
                    DateOfBirth = user.dob.date,
                    Phone = user.phone,
                    Address = $"{user.location.street.number} {user.location.street.name}, {user.location.city}, {user.location.state}, {user.location.country}",
                    ProfilePicture = user.picture.large
                });
            }
            return usersList;
        }
        catch (HttpRequestException e) {
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
    }
}
