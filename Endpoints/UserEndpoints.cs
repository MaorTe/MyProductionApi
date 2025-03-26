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
        .WithSummary("Retrieve a list of users")
        .WithDescription("Retrieves a number of user records from the system.")
        .WithTags("Users");

        app.MapGet("/users/{id:Guid}", async (string id, IUserService service) => {
            var user = await service.GetUserById(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        })
        .WithName("GetUserById")
        .WithSummary("Retrieve a user by ID")
        .WithDescription("Retrieves a single user record by its unique identifier (GUID).")
        .WithTags("Users");

        app.MapGet("/users/search", async ([FromQuery(Name = "query")] string searchTerm, IUserService service) =>
        {
            var matchingUsers = await service.SearchUsers(searchTerm);
            return matchingUsers.Any()
                ? Results.Ok(matchingUsers)
                : Results.NotFound("No users found matching the search criteria.");
        })
        .WithName("SearchUsers")
        .WithSummary("Search for users")
        .WithDescription("Searches for users by matching the provided query string against name, email, phone, or address.")
        .WithTags("Users");

        app.MapPost("/users", async (User user, IUserService service) => {
            var createdUser = await service.CreateUser(user);
            return Results.Created($"/users/{createdUser.Id}", createdUser);
        })
        .WithName("CreateUser")
        .WithSummary("Create a new user")
        .WithDescription("Creates a new user record in the database and returns the newly created user with its generated ID.")
        .WithTags("Users");
    }
}

