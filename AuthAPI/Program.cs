using AuthAPI.AsyncDataService;
using AuthAPI.Services;
using AuthAPI.Services.Interfaces;
using Core.Configuration;
using Core.Entities.Models;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Infrastructure.Configuration;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// JWT
builder.Services.AddSingleton<JwtSettings>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.ConfigureJwt(new JwtSettings(builder.Configuration));

// Cors and swagger
builder.Services.ConfigureCors();
builder.Services.ConfigureSwagger();

// Database
builder.Services.ConfigureMongo(builder.Configuration);
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

// Model services
builder.Services.AddScoped<IUserService, UserService>();

// Tool services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

builder.Services.AddScoped<IMessageBusAuthClient, MessageBusAuthClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seed database
    SeedDatabase<User>.Seed(app);
}

app.UseHttpsRedirection();

app.UseHealthChecks("/healthcheck");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();