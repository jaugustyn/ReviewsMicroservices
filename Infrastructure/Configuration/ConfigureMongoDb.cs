using System.Text.Json.Serialization;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Infrastructure.Configuration;

public static class ConfigureMongoDb
{
    public static void ConfigureMongo(this IServiceCollection services, IConfiguration configuration)
    {
        BsonSerializerRegisterer.RegisterSerializers();

        services.AddHealthChecks().AddCheck<MongoHealthCheck>("MongoDbConnectionCheck");

        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        
        services.AddSingleton<IMongoDbSettings>(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);
        services.AddSingleton<MongoDbContext>();

        services.AddControllers().AddJsonOptions(options =>
        {
            // serialize enums as strings in api responses (e.g. Role)
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }
}

public static class BsonSerializerRegisterer
{
    static BsonSerializerRegisterer()
    {
        try
        {
            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(typeof(DateTimeOffset), new DateTimeOffsetSerializer(BsonType.String));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing BsonSerializerRegisterer: {ex.Message}");
        }
    }

    public static void RegisterSerializers()
    {
    }
}