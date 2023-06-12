using Core.Configuration;
using Core.Entities.Models;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Infrastructure.Configuration;
using Infrastructure.Data;
using Infrastructure.Repositories;
using ReviewsAPI.AsyncDataService;
using ReviewsAPI.EventProcessing;
using ReviewsAPI.Services;
using ReviewsAPI.Services.Interfaces;
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
builder.Services.AddTransient<IReviewRepository, ReviewRepository>();
builder.Services.AddTransient<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddTransient<IEventProcessor, EventProcessor>();

// DataService
builder.Services.AddScoped<IMessageBusReviewClient, MessageBusReviewClient>();
builder.Services.AddHostedService<MessageBusReviewSubscriber>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seed database
    SeedDatabase<Review>.Seed(app);
    SeedDatabase<Rating>.Seed(app);
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseHealthChecks("/healthcheck");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Required to integration tests
public partial class Program { }