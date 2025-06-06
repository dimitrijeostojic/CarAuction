using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//add rules for authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServiceUrl"];
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters.ValidateAudience = false; // Disable audience validation for simplicity
    options.TokenValidationParameters.NameClaimType = "username"; // Set the claim type for username

});

var app = builder.Build();

app.MapReverseProxy();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
