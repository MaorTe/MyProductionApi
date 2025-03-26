using MyProductionApi.Models;
using MyProductionApi.Services;

namespace MyProductionApi.Endpoints;
public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app) {

        app.MapGet("/users", async (IUserService service) => {
            var users = await service.GetAllUsers();
            return Results.Ok(users);
        })
            .WithName("GetUsers")
            .WithSummary("Blabla")
            .WithDescription("Description")
            .WithTags("Tags");

        app.MapGet("/users/{id:Guid}", async (Guid id, IUserService service) => {
            var user = await service.GetUserById(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        app.MapPost("/users", async (User user, IUserService service) => {
            var createdUser = await service.CreateUser(user);
            return Results.Created($"/users/{createdUser.Id}", createdUser);
        });
    }
}

