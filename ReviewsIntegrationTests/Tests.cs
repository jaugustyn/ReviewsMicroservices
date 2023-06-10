using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using ReviewsAPI.Dto.Review;
using Xunit;
using Xunit.Abstractions;

namespace ReviewsIntegrationTests;

public class Tests: IClassFixture<MongoDbFixture>, IClassFixture<CustomWebAppFactory<Program>>
{
    private readonly MongoDbFixture _mongoDbFixture;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    
    public Tests(MongoDbFixture mongoDbFixture, CustomWebAppFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        var projectDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var configPath = Path.Combine(projectDir, "appsettings.json");

        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        
        _mongoDbFixture = mongoDbFixture;
        _testOutputHelper = testOutputHelper;
    }
    
    [Theory]
    [InlineData("/healthcheck")]
    [InlineData("/api/v1/Reviews")]
    public async Task Endpoints_return_success(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);
        response.IsSuccessStatusCode.Should().BeTrue();
    }
    
    [Fact]
    public async Task ReviewsGetAll_ReturnCorrectModels()
    {
        var userId = new Guid("0355d041-a5c1-40ec-9878-330f40c17199");
        var review = new ReviewDto(){Id = new Guid(), Title = "Testowa recenzja", UserId = userId, CreatedAt = DateTimeOffset.Now, Text = "Yey"};

        await _mongoDbFixture.InsertAsync("Reviews", review);
        
        var response = await _client.GetAsync("/api/v1/Reviews");
        
        var contentString = await response.Content.ReadAsStringAsync();
        var reviewsList = JsonConvert.DeserializeObject<List<ReviewDto>>(contentString);

        var addedReview = reviewsList.Find(x => x.Id.Equals(review.Id));
        
        // List should have 3 items, 2 seeded + 1 inserted
        reviewsList.Count.Should().Be(3);
        addedReview.Id.Should().Be(review.Id);
        addedReview.UserId.Should().Be(userId);
        addedReview.Title.Should().Be("Testowa recenzja");
        addedReview.Text.Should().Be("Yey");
        addedReview.CreatedAt.Should().NotBe(null);
    }
    
    [Fact]
    public async Task ReviewsGetOne_ValidData_ReturnCorrectModel()
    {
        var userId = new Guid("0355d041-a5c1-40ec-9878-330f40c17199");
        var review = new ReviewDto(){Id = new Guid(), Title = "Testowa recenzja", UserId = userId, CreatedAt = DateTimeOffset.Now, Text = "Yey"};

        await _mongoDbFixture.InsertAsync("Reviews", review);
        
        var response = await _client.GetAsync($"/api/v1/Reviews/{review.Id}");
        
        var contentString = await response.Content.ReadAsStringAsync();
        var responseReview = JsonConvert.DeserializeObject<ReviewDto>(contentString);
        
        responseReview.Id.Should().Be(review.Id);
        responseReview.UserId.Should().Be(userId);
        responseReview.Title.Should().Be("Testowa recenzja");
        responseReview.Text.Should().Be("Yey");
        responseReview.CreatedAt.Should().NotBe(null);
    }
    
    [Fact]
    public async Task ReviewsGetOne_InvalidReviewId_ReturnNotFound()
    {
        var reviewId = new Guid("0355d041-a5c1-40ec-9878-330f40c17199");
        
        var response = await _client.GetAsync($"/api/v1/Reviews/{reviewId}");
        _testOutputHelper.WriteLine(response.StatusCode.ToString());

        response.StatusCode.ToString().Should().Be("NotFound");
    }
}