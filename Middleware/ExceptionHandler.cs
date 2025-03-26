using Microsoft.AspNetCore.Diagnostics;

namespace MyProductionApi.Middleware;

public static class ExceptionHandler
{
    public static void Configure(WebApplication app) {
        app.UseExceptionHandler("/error");

        app.Map("/error", (HttpContext http) => {
            var feature = http.Features.Get<IExceptionHandlerFeature>();
            var ex = feature?.Error;

            return Results.Problem(
                title: "Unexpected error occurred",
                detail: ex?.Message,
                statusCode: 500);
        });
    }
}