using System.Net;

namespace BarbeariaRocha.Infraestrutura.Middlewares;

public class ExceptionMiddleware
{
    public readonly RequestDelegate _next;
    public readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // passa para o próximo middleware
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var result = new
            {
                status = context.Response.StatusCode,
                message = ex.Message
            };

            await context.Response.WriteAsJsonAsync(result);
        }
    }
}

