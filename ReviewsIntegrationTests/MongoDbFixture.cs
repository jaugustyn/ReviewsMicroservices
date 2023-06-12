using Core.Entities.Models;
using MongoDB.Driver;

namespace ReviewsIntegrationTests;

public class MongoDbFixture : IDisposable
{
    private readonly IMongoClient _mongoClient;
    private readonly IMongoDatabase _mongoDatabase;
    private readonly IMongoCollection<Review> _collection;
    private const string CollectionName = "Reviews";
    private const string DatabaseName = "Reviews_integration_tests";

    public MongoDbFixture()
    {
        _mongoClient = new MongoClient("mongodb://localhost:27017");
        _mongoDatabase = _mongoClient.GetDatabase(DatabaseName);
        _collection = _mongoDatabase.GetCollection<Review>(CollectionName);
    }

    public Task InsertAsync<TEntity>(string collectionName, TEntity entity)
    {
        return _mongoDatabase.GetCollection<TEntity>(collectionName).InsertOneAsync(entity);
    }

    public void Dispose()
    {
        _mongoDatabase.Client.DropDatabase(DatabaseName);
    }
}