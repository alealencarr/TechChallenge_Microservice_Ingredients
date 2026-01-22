using Shared.Result;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

namespace API.Extensions.Middlewares
{
    [ExcludeFromCodeCoverage]

    public class UnauthorizedTokenMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                const string dataType = @"application/problem+json";
                const int statusCode = StatusCodes.Status401Unauthorized;

                var commandResult = await CommandResult.FailAsync($"Token inválido, acesso negado.");

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = dataType;

                await context.Response.WriteAsync(JsonSerializer.Serialize(commandResult,
                    new JsonSerializerOptions { WriteIndented = true }));
            }
            else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                const string dataType = @"application/problem+json";
                const int statusCode = StatusCodes.Status403Forbidden;

                var commandResult = await CommandResult.FailAsync($"Você não tem acesso a esse recurso!");

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = dataType;

                await context.Response.WriteAsync(JsonSerializer.Serialize(commandResult, new JsonSerializerOptions { WriteIndented = true }));
            }
        }
    }
}
