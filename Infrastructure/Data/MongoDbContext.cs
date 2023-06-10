using Core.Entities.Models;
using Infrastructure.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Data;

public class MongoDbContext : IMongoDbContext
{
    private IMongoDatabase Database { get; set; }
    private MongoClient MongoClient { get; set; }
    
    private const string ReviewCollectionName = "Reviews";
    private const string RatingCollectionName = "Ratings";
    private const string CommentCollectionName = "Comments";
    private const string UserCollectionName = "Users";
    private const string RefreshTokenCollectionName = "RefreshTokens";

    public MongoDbContext(IMongoClient mongoClient, IMongoDbSettings mongoDbSettings)
    {
        MongoClient = new MongoClient(mongoDbSettings.ConnectionString);
        Database = MongoClient.GetDatabase(mongoDbSettings.DatabaseName);

        Reviews = Database.GetCollection<Review>(ReviewCollectionName);
        Ratings = Database.GetCollection<Rating>(RatingCollectionName);
        Comments = Database.GetCollection<Comment>(CommentCollectionName);
        Users = Database.GetCollection<User>(UserCollectionName);
        RefreshTokens = Database.GetCollection<RefreshToken>(RefreshTokenCollectionName);
    }

    public IMongoCollection<Review> Reviews { get; }
    public IMongoCollection<Rating> Ratings { get; }
    public IMongoCollection<Comment> Comments { get; }
    public IMongoCollection<User> Users { get; }
    public IMongoCollection<RefreshToken> RefreshTokens { get; }
    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return Database.GetCollection<T>(name);
    }
}