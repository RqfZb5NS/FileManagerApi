using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FileManagerApi.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(
        RequestDelegate next,
        ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var request = await FormatRequest(context.Request);
            _logger.LogInformation("Request: {Request}", request);

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var startTime = DateTime.UtcNow;
            await _next(context);
            var elapsed = DateTime.UtcNow - startTime;

            var response = await FormatResponse(context.Response);
            _logger.LogInformation("Response: {Response} | Time: {Elapsed}ms", 
                response, elapsed.TotalMilliseconds.ToString("0"));

            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            throw;
        }
    }

    private static async Task<string> FormatRequest(HttpRequest request)
    {
        request.EnableBuffering();
        var body = await new StreamReader(request.Body)
            .ReadToEndAsync();
        request.Body.Position = 0;

        return $"{request.Method} {request.Path}{request.QueryString} {body}";
    }

    private static async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        
        return $"HTTP {response.StatusCode} {body}";
    }
}