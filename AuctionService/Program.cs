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
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Log to txt file
//var logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .WriteTo.File("Logs/WebShopLogs.txt", rollingInterval: RollingInterval.Day)
//    .MinimumLevel.Warning()
//    .CreateLogger();

//This is required
builder.Logging.ClearProviders();
//builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(options =>
//{
//options.SwaggerDoc("v1", new OpenApiInfo { Title = "WebShop API", Version = "v1" });
//options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
//{
//Name = "Authorization",
//In = ParameterLocation.Header,
//Type = SecuritySchemeType.ApiKey,
//Scheme = JwtBearerDefaults.AuthenticationScheme
//});

//options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference=new OpenApiReference
//                {
//                    Type=ReferenceType.SecurityScheme,
//                    Id=JwtBearerDefaults.AuthenticationScheme
//                },
//                Scheme = "Oauth2",
//                Name = JwtBearerDefaults.AuthenticationScheme,
//        In = ParameterLocation.Header,
//            },
//            new List<string>()
//        }
//    });

//});
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
        cfg.ConfigureEndpoints(context);
    });
});


//dodajemo konekcione stringove iz konfiguracije
builder.Services.AddDbContext<AuctionDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//services
builder.Services.AddScoped<IAuctionsService, AuctionsService>();


//builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();

//builder.Services.AddIdentityCore<ApplicationUser>() //konfiguracija identity servisa
//   .AddRoles<IdentityRole>() //dodavanje podrske za role
//   .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("WebShop") //dodavanje token provajdera
//   .AddEntityFrameworkStores<WebShopAuthDbContext>() //podesavanje entity framework skladista
//   .AddDefaultTokenProviders(); //dodavanje podrazumevanih token provajdera

//konfiguracija opcija identiteta
//builder.Services.Configure<IdentityOptions>(options =>
//{
//postavke za lozinku korisnika
//options.Password.RequireDigit = false;
//options.Password.RequireLowercase = false;
//options.Password.RequireNonAlphanumeric = false;
//options.Password.RequireUppercase = false;
//options.Password.RequiredLength = 6;
//options.Password.RequiredUniqueChars = 1;

//options.Lockout.AllowedForNewUsers = true;
//options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//options.Lockout.MaxFailedAccessAttempts = 5;

//options.User.RequireUniqueEmail = false;
//});

//builder.Services.ConfigureApplicationCookie(options =>
//{
//options.Cookie.HttpOnly = true;
//options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
//options.LoginPath = "/Identity/Account/Login";
//options.AccessDeniedPath = "/Identity/Account/AccessDenied";
//options.SlidingExpiration = true;
//});

//dodajemo pravila za autentikaciju
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServiceUrl"];
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters.ValidateAudience = false; // Disable audience validation for simplicity
    options.TokenValidationParameters.NameClaimType="username"; // Set the claim type for username

});

//cors konfiguracija
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
