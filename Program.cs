using C_.Filters;
using C_.Middlewares;
using C_.Application.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using AutoMapper;
using C_.Application.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// 🔥 ЛОГИРОВАНИЕ
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// --- 1. Реєстрація сервісів ---

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilter>();
})
.AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopContext>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddMemoryCache();

builder.Services.AddAutoMapper(typeof(MappingProfile));
// --- АУТЕНТИФІКАЦІЯ ТА АВТОРИЗАЦІЯ ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => 
    {
        options.LoginPath = "/api/auth/unauthorized"; 
        options.AccessDeniedPath = "/api/auth/forbidden"; 
    });

builder.Services.AddAuthorization(options =>
{
    // Створюємо політику: дозволено тільки тим, у кого є claim "permission" = "edit"
    options.AddPolicy("CanEdit", policy =>
        policy.RequireClaim("permission", "edit"));
});
var app = builder.Build();

// --- 2. Pipeline ---

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();


// --- Middleware ---
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