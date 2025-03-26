using Microsoft.EntityFrameworkCore;
using MyProductionApi.Data;
using MyProductionApi.Models;
using System.Net.Http;
using System.Text.Json;

namespace MyProductionApi.Services;

public class UserService : IUserService
{
    //private readonly AppDbContext _db;
    private readonly ICacheService _cache;
    private const int CacheTtlSeconds = 300; // Cache Time-to-live (e.g., 5 minutes)


    public UserService(ICacheService cache) {
        _cache = cache;
    }

    public async Task<List<User>> GetAllUsers(int pageSize) {
        List<User> cachedUsers = await _cache.GetAsync<List<User>>("users");
        if (cachedUsers != null) return cachedUsers;

        var users = await GetUsersFromApi($"https://randomuser.me/api/?results={pageSize}");
        await _cache.SetAsync("users", users, 600);
        return users;
    }

    /// <summary>
    /// Retrieves a user by id using the "seed" parameter to always return the same user.
    /// </summary>
    public async Task<User?> GetUserById(string id) {

        string cacheKey = $"user_{id}";
        var cachedUser = await _cache.GetAsync<User>(cacheKey);
        if (cachedUser != null) {
            return cachedUser;
        }

        // Use the "seed" parameter so that the same id always returns the same user.
        string url = $"https://randomuser.me/api/?seed={id}&results=1";

        return (await GetUsersFromApi(url)).First();
    }

    public async Task<List<User>> SearchUsers(string searchTerm) {

        string cacheKey = $"users_ {searchTerm}";
        var cachedUsers = await _cache.GetAsync<List<User>>(cacheKey);
        if (cachedUsers != null) {
            return cachedUsers;
        }

        // Use the "seed" parameter so that the same id always returns the same user.
        string url = $"https://randomuser.me/api/?seed={searchTerm}&results=50";

        List<User> usersList = await GetUsersFromApi(url);

        // Additional in‑memory filtering to simulate a “real” search.
        // This filters the list to only include users whose first name, last name, or email contains the term.
        var filteredUsers = usersList.Where(u =>
        u.FirstName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
        u.LastName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
        u.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true
        ).ToList();

        await _cache.SetAsync(cacheKey, filteredUsers, CacheTtlSeconds);
        return filteredUsers;
    }

    public async Task<User> CreateUser(User user) {
        //_cache.Users.Add(user);
        //await _cache.SaveChangesAsync();
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
            foreach (var user in result) {
                usersList.Add(new User {
                    Id = Guid.Parse(user.login.uuid),
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
