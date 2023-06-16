using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewsAPI.Dto.Rating;
using ReviewsAPI.Dto.Review;
using ReviewsAPI.Services.Interfaces;

namespace ReviewsAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;
    private readonly IReviewService _reviewService;

    public RatingsController(IRatingService ratingService, IReviewService reviewService)
    {
        _ratingService = ratingService;
        _reviewService = reviewService;
    }

    [HttpGet("{reviewId:Guid}")]
    public async Task<ActionResult<IEnumerable<RatingDto>>> GetReviewAverageRating(Guid reviewId)
    {
        var avgRating = await _ratingService.GetRatingsByReviewIdAsync(reviewId);
        if (avgRating is null)
            return NotFound(new {error_message = "This review has no ratings."});

        return Ok(new {rating = avgRating});
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReviewDto>> CreateRating(RatingCreateDto ratingCreateDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var checkReview = _reviewService.GetByIdAsync(ratingCreateDto.ReviewId);
        if (checkReview is null) 
            return NotFound(new {error_message = "Review not found."});

        var newRating = await _ratingService.CreateAsync(userId, ratingCreateDto);

        if (newRating is null)
            return Conflict(new {error_message = "You cannot rate single review multiple times."});

        return Ok(newRating);
    }

    [HttpPut("{ratingId:guid}")]
    [Authorize]
    public async Task<IActionResult> PutRating(Guid ratingId, RatingUpdateDto ratingUpdateDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var role = User.FindFirstValue(ClaimTypes.Role);

        var rating = await _ratingService.GetByIdAsync(ratingId);

        if (rating is null) return NotFound();

        if (!rating.UserId.Equals(userId) && role != "Administrator")
            return Unauthorized(new {error_message = "The rating does not belong to this user."});

        await _ratingService.UpdateAsync(ratingId, ratingUpdateDto);

        return NoContent();
    }

    [HttpDelete("{ratingId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteRating(Guid ratingId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var role = User.FindFirstValue(ClaimTypes.Role);

        var rating = await _ratingService.GetByIdAsync(ratingId);

        if (rating is null) return NotFound();
        if (!rating.UserId.Equals(userId) && role != "Administrator")
            return Unauthorized(new {error_message = "The rating does not belong to this user"});

        await _ratingService.DeleteAsync(ratingId);

        return NoContent();
    }
}