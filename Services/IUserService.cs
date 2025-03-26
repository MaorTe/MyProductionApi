﻿using MyProductionApi.Models;

namespace MyProductionApi.Services;
public interface IUserService
{
    Task<List<User>> GetAllUsers(int pageSize);
    Task<User> GetUserById(Guid id);
    Task<List<User>> SearchUsers(string searchTerm);
    Task<User> CreateUser(User user);
}

