using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using ReviewsAPI.AsyncDataService;
using ReviewsAPI.Controllers;
using ReviewsAPI.Dto.Review;
using ReviewsAPI.Services;

namespace Tests.Controllers;

public class ReviewControllerTests
{
    private readonly ReviewsController _controller;
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly Mock<IMessageBusReviewClient> _messageBusClient;

    public ReviewControllerTests()
    {
        _reviewServiceMock = new Mock<IReviewService>();
        _messageBusClient = new Mock<IMessageBusReviewClient>();
        _controller = new ReviewsController(_reviewServiceMock.Object, _messageBusClient.Object);
    }

    #region GetAll tests

    [Fact]
    public async Task GetAllReviews_ReturnsOkWithAllReviews()
    {
        // Arrange
        var expectedReviews = new List<ReviewDto>
        {
            new() { Id = Guid.NewGuid(), Title = "Review 1", Text = "Text 1", UserId = Guid.NewGuid(), CreatedDate = DateTimeOffset.Now },
            new() { Id = Guid.NewGuid(), Title = "Review 2", Text = "Text 2", UserId = Guid.NewGuid(), CreatedDate = DateTimeOffset.Now },
            new() { Id = Guid.NewGuid(), Title = "Review 3", Text = "Content 3", UserId = Guid.NewGuid(), CreatedDate = DateTimeOffset.Now }
        };

        _reviewServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(expectedReviews);

        // Act
        var result = await _controller.GetAllReviews();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<ReviewDto>>>(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var actualReviews = Assert.IsAssignableFrom<IEnumerable<ReviewDto>>(okObjectResult.Value);

        Assert.Equal(expectedReviews.Count, actualReviews.Count());
        Assert.Equal(expectedReviews, actualReviews);
        // Add more assertions as per your requirements
    }

    #endregion

    #region PostReview tests

    [Fact]
    public async Task Review_ValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var reviewCreateDto = new ReviewCreateDto {Title = "New Review", Text = "Review content"};
        var newReview = new ReviewDto { Id = Guid.NewGuid(), Title = "New Review", Text = "Review content", UserId = userId, CreatedDate = DateTimeOffset.Now };

        _reviewServiceMock.Setup(service => service.CreateAsync(userId, reviewCreateDto)).ReturnsAsync(newReview);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuthentication"))
            }
        };

        // Act
        var result = await _controller.PostReview(reviewCreateDto);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ReviewDto>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var createdReview = Assert.IsAssignableFrom<ReviewDto>(createdAtActionResult.Value);

        Assert.Equal(newReview.Id, createdReview.Id);
        Assert.Equal(newReview.Title, createdReview.Title);
        Assert.Equal(newReview.Text, createdReview.Text);
        Assert.Equal(newReview.UserId, createdReview.UserId);
        Assert.Equal(newReview.CreatedDate, createdReview.CreatedDate);
    }

    #endregion

    #region GetById tests

    [Fact]
    public async Task GetReview_ValidId_ReturnsOkWithReview()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var expectedReview = new ReviewDto { Id = reviewId, Title = "Test Review", Text = "Text 1", UserId = Guid.NewGuid(), CreatedDate = DateTimeOffset.Now };

        _reviewServiceMock.Setup(service => service.GetByIdAsync(reviewId)).ReturnsAsync(expectedReview);

        // Act
        var result = await _controller.GetReview(reviewId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ReviewDto>>(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var actualReview = Assert.IsAssignableFrom<ReviewDto>(okObjectResult.Value);

        Assert.Equal(expectedReview.Id, actualReview.Id);
        Assert.Equal(expectedReview.Title, actualReview.Title);
        // Add more assertions as per your requirements
    }

    [Fact]
    public async Task GetReview_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidReviewId = Guid.NewGuid();

        _reviewServiceMock.Setup(service => service.GetByIdAsync(invalidReviewId));

        // Act
        var result = await _controller.GetReview(invalidReviewId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ReviewDto>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    #endregion

    #region PutReview tests

    [Fact]
    public async Task PutReview_ValidId_ReturnsNoContent()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var reviewUpdateDto = new ReviewUpdateDto() {Title = "Updated Review", Text = "Updated content"};

        var review = new ReviewDto { Id = reviewId, Title = "Old Review", Text = "Old content", UserId = userId, CreatedDate = DateTimeOffset.Now };

        _reviewServiceMock.Setup(service => service.GetByIdAsync(reviewId)).ReturnsAsync(review);
        _reviewServiceMock.Setup(service => service.UpdateAsync(reviewId, reviewUpdateDto)).Returns(Task.CompletedTask);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuthentication"))
            }
        };

        // Act
        var result = await _controller.PutReview(reviewId, reviewUpdateDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PutReview_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidReviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var reviewUpdateDto = new ReviewUpdateDto() {Title = "Updated Review", Text = "Updated content"};

        _reviewServiceMock.Setup(service => service.GetByIdAsync(invalidReviewId)).ReturnsAsync(null as ReviewDto);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuthentication"))
            }
        };

        // Act
        var result = await _controller.PutReview(invalidReviewId, reviewUpdateDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region DeleteReview tests

    [Fact]
    public async Task DeleteReview_ValidId_ReturnsNoContent()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var review = new ReviewDto
        {
            Id = reviewId, Title = "Review to delete", Text = "Content to delete", UserId = userId,
            CreatedDate = DateTimeOffset.Now
        };

        _reviewServiceMock.Setup(service => service.GetByIdAsync(reviewId)).ReturnsAsync(review);
        _reviewServiceMock.Setup(service => service.DeleteAsync(reviewId)).Returns(Task.CompletedTask);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuthentication"))
            }
        };

        // Act
        var result = await _controller.DeleteReview(reviewId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteReview_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidReviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _reviewServiceMock.Setup(service => service.GetByIdAsync(invalidReviewId)).ReturnsAsync(null as ReviewDto);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }, "TestAuthentication"))
            }
        };

        // Act
        var result = await _controller.DeleteReview(invalidReviewId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion
}