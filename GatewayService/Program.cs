using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/GatewayServiceLog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog((ctx, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("Logs/GatewayLog.txt", rollingInterval: RollingInterval.Day)
        .ReadFrom.Configuration(ctx.Configuration); // koristi appsettings ako želiš
});

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//add rules for authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServiceUrl"];
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters.ValidateAudience = false; // Disable audience validation for simplicity
    options.TokenValidationParameters.NameClaimType = "username"; // Set the claim type for username
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["IdentityServiceUrl"];
});

var app = builder.Build();

app.UseSerilogRequestLogging(); // <-- log every http requests automatically
app.MapReverseProxy();
app.UseAuthentication();
app.UseAuthorization();


app.Run();
