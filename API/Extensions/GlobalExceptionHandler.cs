using Microsoft.AspNetCore.Diagnostics;
using Shared.Result;
using System.Diagnostics.CodeAnalysis;

namespace API.Extensions
{
    [ExcludeFromCodeCoverage]

    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception,
                $"Exceção não tratada: {exception.InnerException?.Message ?? exception.Message}");

            var result = CommandResult.Fail(exception.InnerException?.Message ?? exception.Message);
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);

            return true;
        }
    }
}
