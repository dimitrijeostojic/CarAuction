//using AuctionService.Data;
//using AuctionService.Mappings;
//using Microsoft.EntityFrameworkCore;

using AuctionService.Consumers;
using AuctionService.Data;
using AuctionService.Mappings;
using AuctionService.Middlewares;
using AuctionService.Services.Implementations;
using AuctionService.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Log to txt file
var logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Console()
    .WriteTo.File("Logs/AuctionServiceLog.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

//This is required
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

//adding masstransit for producer
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(5);
        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            h.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
});


//add conn strings from configuration
builder.Services.AddDbContext<AuctionDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//services
builder.Services.AddScoped<IAuctionsService, AuctionsService>();


//builder.Services.ConfigureApplicationCookie(options =>
//{
//options.Cookie.HttpOnly = true;
//options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
//options.LoginPath = "/Identity/Account/Login";
//options.AccessDeniedPath = "/Identity/Account/AccessDenied";
//options.SlidingExpiration = true;
//});

//add rules for authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServiceUrl"];
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters.ValidateAudience = false; // Disable audience validation for simplicity
    options.TokenValidationParameters.NameClaimType="username"; // Set the claim type for username
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["IdentityServiceUrl"]; // Set the valid issuer

});

//cors configuration
builder.Services.AddCors(options =>
{
options.AddPolicy("AllowAllOrigins",
    policy =>
{
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
});
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

//we have to add this middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
