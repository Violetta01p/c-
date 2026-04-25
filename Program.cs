using C_.Application.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using System.Diagnostics;
using Microsoft.AspNetCore.Http; 
using System.Threading.Tasks;    
using System;
using AutoMapper;
// Якщо ти створила папку Application, додай ці рядки:
using C_.Application.DTOs;



var builder = WebApplication.CreateBuilder(args);

// --- 1. Реєстрація сервісів ---
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopContext>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));


var app = builder.Build();

// --- 2. Налаштування Pipeline ---

// Використовуємо Middleware
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();

// --- 3. Клас Middleware (тепер точно без помилок) ---
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        await _next(context); 

        stopwatch.Stop();
        
        Console.WriteLine($"[LOG] {DateTime.Now:HH:mm:ss} | {context.Request.Method} {context.Request.Path} | {context.Response.StatusCode} | {stopwatch.ElapsedMilliseconds}ms");
    }
}

