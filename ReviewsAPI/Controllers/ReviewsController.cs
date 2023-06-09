﻿using System.Security.Claims;
using Infrastructure.AsyncDataServices.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewsAPI.AsyncDataService;
using ReviewsAPI.Dto.Review;
using ReviewsAPI.Services.Interfaces;

namespace ReviewsAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IMessageBusReviewClient _messageBusClient;
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService, IMessageBusReviewClient messageBusClient)
    {
        _reviewService = reviewService;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviews()
    {
        return Ok(await _reviewService.GetAllAsync());
    }

    [HttpGet("{search}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> SearchReviews(string search)
    {
        var result = await _reviewService.SearchAsync(search);

        if (result.Any()) return Ok(result);

        return NotFound();
    }

    [HttpGet("{reviewId:guid}")]
    public async Task<ActionResult<ReviewDto>> GetReview(Guid reviewId)
    {
        var review = await _reviewService.GetByIdAsync(reviewId);
        if (review is null) return NotFound();

        return Ok(review);
    }

    [HttpGet("GetMyReviews")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetMyReviews()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var reviews = await _reviewService.GetAllByUserIdAsync(userId);
        if (reviews is null) return NotFound();

        return Ok(reviews);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReviewDto>> PostReview(ReviewCreateDto reviewCreateDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var newReview = await _reviewService.CreateAsync(userId, reviewCreateDto);

        return CreatedAtAction(nameof(GetReview), new {reviewId = newReview.Id}, newReview);
    }

    [HttpPut("{reviewId:guid}")]
    [Authorize]
    public async Task<IActionResult> PutReview(Guid reviewId, ReviewUpdateDto reviewUpdateDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var role = User.FindFirstValue(ClaimTypes.Role);

        var review = await _reviewService.GetByIdAsync(reviewId);

        if (review is null) return NotFound();

        if (!review.UserId.Equals(userId) && role != "Administrator")
            return Unauthorized(new {error_message = "The review does not belong to this user"});

        await _reviewService.UpdateAsync(reviewId, reviewUpdateDto);

        return NoContent();
    }

    [HttpDelete("{reviewId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(Guid reviewId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var role = User.FindFirstValue(ClaimTypes.Role);

        Console.WriteLine(role);

        var review = await _reviewService.GetByIdAsync(reviewId);

        if (review is null) return NotFound();
        if (!review.UserId.Equals(userId) && role != "Administrator")
            return Unauthorized(new {error_message = "The review does not belong to this user"});

        await _reviewService.DeleteAsync(reviewId);
        var publishDeleteReviewDto = new ReviewDeletedPublisherDto(reviewId, "Review_Deleted");

        try
        {
            _messageBusClient.PublishReviewDeleteEvent(publishDeleteReviewDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
        }

        return NoContent();
    }
}