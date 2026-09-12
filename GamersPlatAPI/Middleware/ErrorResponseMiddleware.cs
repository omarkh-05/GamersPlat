using System.Text.Json;
namespace GamersPlatAPI.Middleware
{
    public class ErrorResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorResponseMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var payload = new { success = false, message = "Server error", error = ex.Message };
                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        }
    }
}
