using System.Net;
using System.Text.Json;

namespace GamersPlatAPI.Middleware
{
    public class ErrorResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorResponseMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = ex switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Forbidden,
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                statusCode = statusCode,
                message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
