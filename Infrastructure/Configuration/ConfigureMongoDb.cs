using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Infrastructure.Configuration;

public static class ConfigureMongoDb
{
    public static void ConfigureMongo(this IServiceCollection services, MongoDbSettings mongoDbSettings)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient(mongoDbSettings.ConnectionString));
    }
}