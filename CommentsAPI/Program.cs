using System.Text.Json.Serialization;
using CommentsAPI.AsyncDataService;
using CommentsAPI.EventProcessing;
using CommentsAPI.Services;
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
builder.Services.ConfigureJwt(new JwtSettings(builder.Configuration));
builder.Services.AddScoped<IJwtService, JwtService>();

// Cors and swagger
builder.Services.ConfigureCors();
builder.Services.ConfigureSwagger();

// Database
builder.Services.ConfigureMongo(builder.Configuration);
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddTransient<IEventProcessor, EventProcessor>(); 

// DataService
builder.Services.AddHostedService<MessageBusCommentSubscriber>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Seed database
    SeedDatabase<Comment>.Seed(app);
}

app.UseHttpsRedirection();

app.UseHealthChecks("/healthcheck");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();