using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
 //Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CookieAuthenticationOptions CookieAuthenticationHandler
builder.Services.AddAuthentication()
    .AddCookie("local");



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

app.MapGet("/", context => Task.FromResult("Hello world"));

app.MapGet("/login-local", async (context) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr","anton"));

    var identity = new ClaimsIdentity(claims,"local");

    var user = new ClaimsPrincipal(identity);

    await context.SignInAsync("local",user);
});


app.MapControllers();

app.Run();


public class VsitorHandler : CookieAuthenticationHandler
{
    public VsitorHandler(IOptionsMonitor<CookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = await base.HandleAuthenticateAsync();

        if (result.Succeeded)
        {
            return result;
        }
        var claims = new List<Claim>();
        claims.Add(new Claim("usr", "anton"));

        var identity = new ClaimsIdentity(claims, "visitor");

        var user = new ClaimsPrincipal(identity);

         await Context.SignInAsync("visitor", user);

        return AuthenticateResult.Success(new AuthenticationTicket(user, "visitor"));

    }
}