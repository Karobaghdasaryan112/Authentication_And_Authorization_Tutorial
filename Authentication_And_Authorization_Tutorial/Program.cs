using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//who is authenticated you
builder.Services.AddAuthentication("cookie").AddCookie("cookie");

builder.Services.AddAuthorization();
//Encrypt and Decrypt datas for secure
//Encrypt and Decrypt the cookies
builder.Services.AddDataProtection();

////Accessor for HttpContext what I use for my AuthService class
builder.Services.AddHttpContextAccessor();


//Inject an Auth Service
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}






app.Use((context, next) =>
{
    var protectionProvider = context.RequestServices.GetService<IDataProtectionProvider>();

    string secretProtectorName = "auth-cookie";

    var Protector = protectionProvider.CreateProtector(secretProtectorName);

    var authCookie = context.Request.Headers.Cookie.FirstOrDefault(C => C.StartsWith("auth="));
    if (authCookie != null)
    {
        var ProtectedPayload = authCookie.Split("=").Last();

        var Payload = Protector.Unprotect(ProtectedPayload);

        var Parts = Payload.Split(":");
        var Key = Parts[0];
        var Value = Parts[1];

        var claims = new List<Claim>()
    {
        new Claim(Key, Value)
    };

        var identity = new ClaimsIdentity(claims);

        var Principal = new ClaimsPrincipal(identity);

        context.User = Principal;
    }


    return next();
});

//Equivalent to my custom Authentication middleware
app.UseAuthentication();
app.UseAuthorization();

//For Secure SaimSiteMode HttpOnlyCookie, success within server
app.UseCookiePolicy();
app.MapGet("/UserName", (HttpContext context) =>
{
    if (context.User?.Identity?.IsAuthenticated != true)
        return Results.Unauthorized();

    return Results.Ok($"{context.User.FindFirstValue(ClaimTypes.Name)}");

}).RequireAuthorization(new AuthorizeAttribute() { Roles = "Admin" });

app.MapGet("/Login", async (HttpContext context) =>
{
    var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name,"anton"),
            new Claim(ClaimTypes.Role,"User")
        };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    var Principal = new ClaimsPrincipal(identity);

    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, Principal);

    return "Ok";
});

app.MapGet("/unsecure", (HttpContext context) =>
{
    var AuthScheme = "cookie";

    if (!context.User.Identities.Any(x => x.AuthenticationType == AuthScheme))
    {
        context.Response.StatusCode = 401;
        return "empty";
    }

    if (!context.User.HasClaim("password_Type","eur"))
    {
        context.Response.StatusCode = 403;
        return "";
    }

    return "allowed";

});

app.MapGet("/Login", async (HttpContext context) =>
{
    var AuthScheme = "cookie";
    var claims = new List<Claim>()
    {
        new Claim("password_Type","eur"),
        new Claim(ClaimTypes.Name,"anton")
    };
    var ClaimsIdentity = new ClaimsIdentity(claims,AuthScheme);

    var ClaimsPrincipal = new ClaimsPrincipal(ClaimsIdentity);

    await context.SignInAsync(AuthScheme, ClaimsPrincipal);

    return "Ok";

});
app.UseHttpsRedirection();

app.MapControllers();

app.Run();



public class AuthService
{
    private readonly IDataProtectionProvider _protectionProvider;
    private readonly IHttpContextAccessor _contextAccessor;
    public AuthService(IDataProtectionProvider protectionProvider, IHttpContextAccessor httpContextAccessor)
    {
        _protectionProvider = protectionProvider;
        _contextAccessor = httpContextAccessor;
    }

    public void SignInAsync()
    {
        string secretProtectorName = "auth-cookie";
        var Protector = _protectionProvider.CreateProtector(secretProtectorName);
        _contextAccessor.HttpContext.Response.Headers.SetCookie = ($"auth={Protector.Protect("usr:anton")}");
    }
}





