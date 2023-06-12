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

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
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
    public async Task<ActionResult<ReviewDto>> AddRating(RatingCreateDto ratingCreateDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

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

        var review = await _ratingService.GetByIdAsync(ratingId);

        if (review is null) return NotFound();

        if (!review.UserId.Equals(userId) && role != "Administrator")
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

        var review = await _ratingService.GetByIdAsync(ratingId);

        if (review is null) return NotFound();
        if (!review.UserId.Equals(userId) && role != "Administrator")
            return Unauthorized(new {error_message = "The review does not belong to this user"});

        await _ratingService.DeleteAsync(ratingId);

        return NoContent();
    }
}