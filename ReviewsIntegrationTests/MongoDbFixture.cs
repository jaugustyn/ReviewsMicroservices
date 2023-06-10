using System.Security.Claims;
using System.Text.Encodings.Web;
using Core.Entities.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
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
        => _mongoDatabase.GetCollection<TEntity>(collectionName).InsertOneAsync(entity);
    
    public void Dispose()
    {
        _mongoDatabase.Client.DropDatabase(DatabaseName);
    }
}


// public MongoDbFixture(WebApplicationFactory<Program> factory)
    // {
    //     var projectDir = Directory.GetCurrentDirectory();
    //     var configPath = Path.Combine(projectDir, "appsettings.json");
    //
    //     _httpClient = factory.WithWebHostBuilder(builder =>
    //     {
    //         builder.ConfigureAppConfiguration((context, conf) =>
    //         {
    //             conf.AddJsonFile(configPath);
    //         });
    //         
    //         builder.ConfigureTestServices(services =>
    //         {
    //             services.RemoveAll(typeof(IHostedService));
    //             services.AddAuthentication(o =>
    //             {
    //                 o.DefaultAuthenticateScheme = "TestScheme";
    //                 o.DefaultChallengeScheme = "TestScheme";
    //             }).AddScheme<AuthenticationSchemeOptions, ReviewsIntegrationTests.TestAuthHandler>("TestScheme", _ => { });
    //         });
    //     }).CreateClient(new WebApplicationFactoryClientOptions()
    //     {
    //         AllowAutoRedirect = false,
    //     });
    //
    //     // HTTP client configuration
    //     _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
    //         
    //         
    //     ConnString = "mongodb://localhost:27017";
    //     DbName = $"test_db_{Guid.NewGuid()}";
    //
    //     var mongoClient = new MongoClient(ConnString);
    //     var db = mongoClient.GetDatabase(DbName);
    //     
    //     DbContext = new MongoDbContext(mongoClient);
    //     // _mongoDatabase = DbContext;
    // }
    //
    // private string ConnString { get; }
    // private string DbName { get; }
    // public MongoDbContext DbContext { get; }

    // public void Dispose()
    // {
    //     var client = new MongoClient(ConnString);
    //     client.DropDatabase(DbName);
    // }
// }

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { 
            new Claim(ClaimTypes.Name, "Test user"), 
            new Claim(ClaimTypes.NameIdentifier, "userId"),
            new Claim(ClaimTypes.Email, "user@example.com")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}