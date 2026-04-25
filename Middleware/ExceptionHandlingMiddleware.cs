
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace C_.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Пропускаємо запит далі
            await _next(context);
        }
        catch (Exception ex)
        {
            // Якщо десь сталася критична помилка — ловимо її тут
            context.Response.StatusCode = 500; // 500 Internal Server Error
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                Error = "Внутрішня помилка сервера (БД або система)",
                Details = ex.Message
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
