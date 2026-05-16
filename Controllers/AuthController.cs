using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace C_.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("login-admin")]
    public async Task<IActionResult> LoginAdmin()
    {
        // Адмін: має роль "Admin" і право "edit"
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "John Admin"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("permission", "edit")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        
        return Ok("Успішний вхід: ADMIN. Вам доступно видалення та створення.");
    }

    [HttpGet("login-user")]
    public async Task<IActionResult> LoginUser()
    {
        // Звичайний користувач: має роль "User", але НЕМАЄ права "edit"
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "Bob User"),
            new Claim(ClaimTypes.Role, "User")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        
        return Ok("Успішний вхід: USER. Вам доступний лише перегляд товарів.");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok("Ви вийшли із системи.");
    }

    // Демонстрація отримання Claims з коду (вимога завдання)
    [Authorize]
    [HttpGet("my-claims")]
    public IActionResult GetMyClaims()
    {
        var userClaims = HttpContext.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(new { User = HttpContext.User.Identity?.Name, Claims = userClaims });
    }

    [HttpGet("unauthorized")]
    public IActionResult NotAuth() => StatusCode(401, new { message = "Помилка 401: Ви не авторизовані!" });

    [HttpGet("forbidden")]
    public IActionResult Forbidden() => StatusCode(403, new { message = "Помилка 403: У вас немає прав для цієї дії!" });
}