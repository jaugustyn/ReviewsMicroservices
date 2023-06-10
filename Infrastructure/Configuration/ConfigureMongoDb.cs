using System.Text.Json.Serialization;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Infrastructure.Configuration;

public static class ConfigureMongoDb
{
    public static void ConfigureMongo(this IServiceCollection services, IConfiguration configuration)
    {
        // https://stackoverflow.com/questions/51270502/how-to-configure-bsonserializer-without-use-of-static-registry
        // Not thread safe, btw doesnt work...
        // if(BsonSerializer.LookupSerializer<Guid>().GetType() != typeof(GuidSerializer))
        // {
        //     BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        //     BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
        // }

        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
        
        services.AddHealthChecks().AddCheck<MongoHealthCheck>("MongoDbConnectionCheck");
        
        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        
        var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
        
        services.AddSingleton<IMongoDbSettings>(serviceProvider => serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value);
        services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient(mongoDbSettings.ConnectionString));
        
        services.AddControllers().AddJsonOptions(options =>
        {
            // serialize enums as strings in api responses (e.g. Role)
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }
    public class CustomSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (type == typeof(Guid)) return new GuidSerializer(BsonType.String);
            if (type == typeof(Guid)) return new DateTimeOffsetSerializer(BsonType.String);

            // fall back to Mongo's defaults
            return null;
        }
    }
}