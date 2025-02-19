using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication().
    AddCookie("default",option =>
    {
        option.Cookie.Name = "MyCookie";
        option.SlidingExpiration = false;
        option.ExpireTimeSpan = TimeSpan.FromSeconds(14);
       
    });


builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/test", () => "Hello World!");

app.MapGet("/Test2", async (HttpContext context) =>
{
    await context.ChallengeAsync("default", new AuthenticationProperties()
    {
        RedirectUri = "Anything-that-we-want"
    });
});

app.MapControllers();

app.Run();
