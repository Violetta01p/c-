using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic; // Потрібно для KeyNotFoundException
using System.Net;
using System.Text.Json;
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
            // Якщо сталася помилка — викликаємо наш метод обробки
            await HandleExceptionAsync(context, ex);
        }
    }


    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // За замовчуванням помилка 500
        var code = HttpStatusCode.InternalServerError; 


        // Перевірка типу помилки для вибору правильного HTTP статусу
        switch (exception)
        {
            case KeyNotFoundException:
                code = HttpStatusCode.NotFound; // 404
                break;


            case ArgumentException:
            case InvalidOperationException:
                code = HttpStatusCode.BadRequest; // 400
                break;


            // Сюди можна додавати інші типи виключень
            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized; // 401
                break;


            default:
                code = HttpStatusCode.InternalServerError; // 500
                break;
        }


        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;


        // Створюємо об'єкт відповіді
        var response = new 
        { 
            error = "Сталася помилка при обробці запиту",
            message = exception.Message,
            status = (int)code,
            // Додаємо деталі внутрішньої помилки, якщо вони є (корисно для розробника)
            details = exception.InnerException?.Message 
        };


        var result = JsonSerializer.Serialize(response);


        return context.Response.WriteAsync(result);
    }
}
