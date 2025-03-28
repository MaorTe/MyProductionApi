using MyProductionApi.Models;

namespace MyProductionApi.Services;
public interface IUserService
{
    Task<List<User>> GetAllUsers(int pageNumber, int pageSize);
    Task<User> GetUserById(string id);
    Task<List<User>> SearchUsers(string searchTerm);
    Task<User> CreateUser(User user);
}

