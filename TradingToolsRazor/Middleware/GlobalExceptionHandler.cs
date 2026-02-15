using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace TradingToolsRazor.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var traceId = httpContext.TraceIdentifier;
            var path = httpContext.Request.Path;
            var method = httpContext.Request.Method;

            _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}, Method: {Method}", traceId, path, method);

            // Determine status code based on exception type
            var statusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            httpContext.Response.StatusCode = statusCode;

            // Store exception in HttpContext Items for Error page to access
            httpContext.Items["Exception"] = exception;
            httpContext.Items["ExceptionStatusCode"] = statusCode;

            // Re-execute to Error page
            var exceptionHandlerFeature = new ExceptionHandlerFeature
            {
                Error = exception,
                Path = path
            };
            httpContext.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);
            httpContext.Features.Set<IExceptionHandlerPathFeature>(exceptionHandlerFeature);

            httpContext.Request.Path = "/Error";
            httpContext.Request.QueryString = new QueryString($"?statusCode={statusCode}");

            await Task.CompletedTask;
            
            // Return false to allow the pipeline to re-execute to Error page
            return false;
        }
    }
}
