using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HrManagement.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private static readonly string LogDirectory = "logs"; // Directory for storing logs

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");

            // Ensure the log directory exists
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            // Generate a unique log file name using DateTime format "ddMMyyyyHHmmssfff"
            string fileName = $"{DateTime.Now:ddMMyyyyHHmmssfff}.json";
            string filePath = Path.Combine(LogDirectory, fileName);

            // Create structured log entry
            var logEntry = new
            {
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Source = ex.Source,
                InnerException = ex.InnerException?.Message,
                Path = context.Request.Path,
                Method = context.Request.Method
            };

            // Write the log entry to a JSON file
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true }));

            // Return a generic error response
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new { error = "An unexpected error occurred. Please try again later." };
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }

}
