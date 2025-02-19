using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using StackExchange.Redis;
using SSO_Authenticatoin_with_cookie.Service;
using SSO_Authenticatoin_with_cookie.Interfaces;
using SSO_Authenticatoin_with_cookie.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SSO_Authenticatoin_with_cookie.Entity;
using SSO_Authenticatoin_with_cookie.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDataProtection();

builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration["DefaultConnection"];

builder.Services.AddDbContext<DataProtectionDbContext>
    (
    option => 
    option.UseSqlServer(connectionString).EnableSensitiveDataLogging()
    );


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
    AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddAuthorization();


builder.Services.AddScoped<IDataProcessingService,DataProcessingService>();

builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => "Hello World! identity");
app.MapGet("/protected",()  => "secret!").RequireAuthorization();
app.MapPost("/Registration", async ([FromBody]User user,IUserService userService) => {

    await userService.UserRegistration(user);

    return "Ok";
});

app.MapGet("/Login", async (HttpContext context, IDataProcessingService processingService, DataProtectionDbContext dbContext, [FromBody]UserLoginViewModel userViewModel) =>
{
    var ExistingUser = await dbContext.Users.Where(U => U.UserName == userViewModel.UserName).FirstOrDefaultAsync();

    if (ExistingUser != null)
    {


        await context.SignInAsync(
             new ClaimsPrincipal(
                 new ClaimsIdentity(
                     new List<Claim>()
                     {
                    new Claim(ClaimTypes.NameIdentifier,await processingService.ProtectData(Guid.NewGuid().ToString()))
                     }, CookieAuthenticationDefaults.AuthenticationScheme
                     )
                 )
             );
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapControllers();

app.Run();
