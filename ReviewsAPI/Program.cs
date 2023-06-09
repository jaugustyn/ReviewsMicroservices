using System.Text.Json.Serialization;
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
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // serialize enums as strings in api responses (e.g. Role)
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();

// JWT
builder.Services.AddSingleton<JwtSettings>();
builder.Services.ConfigureJwt(new JwtSettings(builder.Configuration));
builder.Services.AddScoped<IJwtService, JwtService>();

// Cors and swagger
builder.Services.ConfigureCors();
builder.Services.ConfigureSwagger();

// Database
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
builder.Services.ConfigureMongo(mongoDbSettings);
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddTransient<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
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
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();