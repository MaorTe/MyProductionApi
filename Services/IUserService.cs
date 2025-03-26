using MyProductionApi.Models;

namespace MyProductionApi.Services;
public interface IUserService
{
    Task<List<User>> GetAllUsers();
    Task<User> GetUserById(Guid id);
    Task<User> GetUserByEmail(string email);
    Task<User> CreateUser(User user);

}

