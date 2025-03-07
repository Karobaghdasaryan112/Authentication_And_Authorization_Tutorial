using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSO_Authenticatoin_with_cookie.Data;
using SSO_Authenticatoin_with_cookie.Helpers;
using SSO_Authenticatoin_with_cookie.Interfaces;
using SSO_Authenticatoin_with_cookie.Service;
using SSO_Authenticatoin_with_cookie.ViewModels;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration["DefaultConnection"];
builder.Services.AddDbContext<DataProtectionDbContext>(options =>
    options.UseSqlServer(connectionString).EnableSensitiveDataLogging());

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<DataProtectionDbContext>()
    .SetApplicationName("My_App")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(7));


builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth",option =>
    {
        option.Cookie.Name = "SSO_Cookie";
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World from App 2!");
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
        await context.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, userViewModel.UserName)
        }, CookieAuthenticationDefaults.AuthenticationScheme)));

        return "Login successful!";
    }

    return "BadRequest";
});

app.Run("https://localhost:7062");

