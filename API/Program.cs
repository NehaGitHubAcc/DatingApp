using System.Text;
using API.Data;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService, TokenService>(); // This registers the TokenService class as a service that can be injected into other classes that depend on ITokenService.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("TokenKey is not configured in appsettings.json - Program.cs");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

var app = builder.Build();

// Configure the HTTP request pipeline. (Middlewares) - Order is Important here
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithOrigins("http://localhost:4200","https://localhost:4200"));
app.UseAuthentication(); // This middleware is responsible for authenticating the user based on the JWT token sent in the request. It validates the token and sets the user identity in the HttpContext if the token is valid.
app.UseAuthorization(); // This middleware is responsible for authorizing the user to access specific endpoints based on the user identity set by the authentication middleware and the authorization policies defined in the application. It checks if the user has the necessary permissions to access the requested resource and returns a 403 Forbidden response if the user is not authorized.
app.MapControllers();

app.Run();
