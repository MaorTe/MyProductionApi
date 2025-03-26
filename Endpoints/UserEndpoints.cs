using Microsoft.AspNetCore.Mvc;
using MyProductionApi.Models;
using MyProductionApi.Services;

namespace MyProductionApi.Endpoints;
public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app) {

        app.MapGet("/users", async (IUserService service) => {
            var users = await service.GetAllUsers(10);
            return Results.Ok(users);
        })
        .WithName("GetUsers")
        .WithSummary("Blabla")
        .WithDescription("Description")
        .WithTags("Tags");

        app.MapGet("/users/{id:Guid}", async (string id, IUserService service) => {
            var user = await service.GetUserById(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        })
        .WithName("GetUserById")
        .WithSummary("User Id")
        .WithDescription("Get user id from api")
        .WithTags("Tags");

        app.MapGet("/users/search", async ([FromQuery(Name = "query")] string searchTerm, IUserService service) =>
        {
            var matchingUsers = await service.SearchUsers(searchTerm);
            return matchingUsers.Any()
                ? Results.Ok(matchingUsers)
                : Results.NotFound("No users found matching the search criteria.");
        })
        .WithName("SearchUsers")
        .WithSummary("Search users")
        .WithDescription("Search users by name, email, phone, or address")
        .WithTags("Users");

        app.MapPost("/users", async (User user, IUserService service) => {
            var createdUser = await service.CreateUser(user);
            return Results.Created($"/users/{createdUser.Id}", createdUser);
        })
        .WithName("CreateUser")
        .WithSummary("create user")
        .WithDescription("create user to db")
        .WithTags("Tags");
    }
}

