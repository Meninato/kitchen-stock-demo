namespace KitchenStock.Api.Middlewares;

public class MediaTypeResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MediaTypeResponseMiddleware> _logger;

    public MediaTypeResponseMiddleware(RequestDelegate next, ILogger<MediaTypeResponseMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Capture the original response body stream
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        // Check if it's a 415 response (remove the length check)
        if (context.Response.StatusCode == 415)
        {
            await Handle415ResponseAsync(context, originalBodyStream);
        }
        else
        {
            // Copy the captured response back to the original stream
            context.Response.Body = originalBodyStream;
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task Handle415ResponseAsync(HttpContext context, Stream originalBodyStream)
    {
        // Reset the response
        context.Response.Body = originalBodyStream;
        context.Response.ContentType = "application/json";

        // Clear any existing content
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Cannot modify 415 response - response has already started");
            return;
        }

        var response = new
        {
            message = "The request content type is not supported.",
            metadata = new
            {
                error_code = "UNSUPPORTED_MEDIA_TYPE",
                field = "Content-Type",
                type = "Validation"
            }
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}