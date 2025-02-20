using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSO_Authenticatoin_with_cookie.Data;
using SSO_Authenticatoin_with_cookie.Entity;
using SSO_Authenticatoin_with_cookie.Helpers;
using SSO_Authenticatoin_with_cookie.Interfaces;
using SSO_Authenticatoin_with_cookie.Service;
using SSO_Authenticatoin_with_cookie.ViewModels;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Подключение к базе данных
var connectionString = builder.Configuration["DefaultConnection"];
builder.Services.AddDbContext<DataProtectionDbContext>(options =>
    options.UseSqlServer(connectionString).EnableSensitiveDataLogging());

// Настройка Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionDbContext>()
    .SetApplicationName("My_App");

// Настройка аутентификации и авторизации
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth",options =>
    {
        options.Cookie.Name = "SSO_Cookie";
    });

builder.Services.AddAuthorization();

// Настройка сервисов
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World from App 1!");
app.MapGet("/protected", (HttpContext context) =>
{
    if (!context.User.Identity.IsAuthenticated)
        return "Unauthorized";

    return $"Hello, {context.User.Identity.Name}!";
});

app.MapPost("/Login", async (HttpContext context, DataProtectionDbContext dbContext, [FromBody] UserLoginViewModel userViewModel) =>
{
    var existingUser = await dbContext.Users
        .FirstOrDefaultAsync(u => u.UserName == userViewModel.UserName);

    if (existingUser != null && PasswordHasher.VerifyPassword(userViewModel.Password, existingUser.Password))
    {
        await context.SignInAsync("MyCookieAuth", new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, userViewModel.UserName)
        },
        "MyCookieAuth")),
        new AuthenticationProperties()
        {
            IsPersistent = true
        });

        return "Login successful!";
    }

    return "BadRequest";
});

app.MapPost("/Registration", async (IUserService userService, [FromBody] User user) =>
{
   await  userService.UserRegistration(user);

    return "Ok";
});

app.Run(); // Запуск первого приложения на порту 7062
