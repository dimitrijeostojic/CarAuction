using Microsoft.EntityFrameworkCore.Internal;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Data;
using SearchService.Middlewares;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

//dodajemo autentikaciju u middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
	await SearchDbContext.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();
