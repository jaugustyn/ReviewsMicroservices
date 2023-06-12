using Infrastructure.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Data;

public class MongoHealthCheck : IHealthCheck
{
    private IMongoDatabase Db { get; set; }
    private MongoClient MongoClient { get; set; }

    public MongoHealthCheck(IOptions<MongoDbSettings> configuration)
    {
        MongoClient = new MongoClient(configuration.Value.ConnectionString);
        Db = MongoClient.GetDatabase(configuration.Value.DatabaseName);
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var healthCheckResultHealthy = await CheckMongoDbConnectionAsync();

        if (healthCheckResultHealthy) return HealthCheckResult.Healthy("MongoDB health check success");

        return HealthCheckResult.Unhealthy("MongoDB health check failure");
    }

    private async Task<bool> CheckMongoDbConnectionAsync()
    {
        try
        {
            await Db.RunCommandAsync((Command<BsonDocument>) "{ping:1}");
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}