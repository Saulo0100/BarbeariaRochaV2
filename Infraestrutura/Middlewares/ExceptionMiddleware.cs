using System.Net;

namespace BarbeariaRocha.Infraestrutura.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado na requisição {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var message = env.IsProduction()
                ? "Ocorreu um erro interno. Tente novamente mais tarde."
                : ex.Message;

            await context.Response.WriteAsJsonAsync(new
            {
                status = context.Response.StatusCode,
                message
            });
        }
    }
}

