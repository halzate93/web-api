using System.Text.Json;

namespace UserManagement.Middleware;

public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;
    private const string API_KEY_HEADER = "api-key";

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next, 
        IConfiguration configuration,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication for error endpoint
        if (context.Request.Path.StartsWithSegments("/error"))
        {
            await _next(context);
            return;
        }

        // Check if api-key header exists
        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
        {
            _logger.LogWarning("API key missing from request to {Path}", context.Request.Path);
            await WriteUnauthorizedResponse(context, "Invalid or missing API key");
            return;
        }

        // Get the API key from configuration
        var apiKey = _configuration.GetValue<string>("ApiSettings:ApiKey");

        // Validate the API key
        if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals(extractedApiKey))
        {
            _logger.LogWarning("Invalid API key attempt for {Path}", context.Request.Path);
            await WriteUnauthorizedResponse(context, "Invalid or missing API key");
            return;
        }

        // API key is valid, continue to next middleware
        await _next(context);
    }

    private static Task WriteUnauthorizedResponse(HttpContext context, string message)
    {
        var response = new
        {
            error = message,
            statusCode = StatusCodes.Status401Unauthorized
        };

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var jsonResponse = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(jsonResponse);
    }
}
