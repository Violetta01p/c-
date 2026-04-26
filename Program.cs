using C_.Filters;
using C_.Middlewares;
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
using C_.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Реєстрація сервісів ---

// ЗМІНА 1: Додали реєстрацію ApiExceptionFilter
builder.Services.AddControllers(options => 
{
    options.Filters.Add<ApiExceptionFilter>();
}).AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopContext>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// --- 2. Налаштування Pipeline ---

// ЗМІНА 2: Додали ExceptionHandlingMiddleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Використовуємо існуючий Middleware
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();

// --- 3. Клас Middleware ---
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




