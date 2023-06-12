using System.Security.Claims;
using CommentsAPI.Dto.Comment;
using CommentsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommentsAPI.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetAllComments()
    {
        return Ok(await _commentService.GetAllAsync());
    }

    [HttpGet("{commentId:guid}")]
    public async Task<ActionResult<CommentDto>> GetComment(Guid commentId)
    {
        var comment = await _commentService.GetByIdAsync(commentId);
        if (comment == null) return NotFound();
        return Ok(comment);
    }

    [HttpGet("GetMyComments")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetMyComments()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var comments = await _commentService.GetAllByUserIdAsync(userId);
        if (comments is null) return NotFound();

        return Ok(comments);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CommentDto>> PostComment(CommentCreateDto commentCreateDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var newComment = await _commentService.CreateAsync(userId, commentCreateDto);

        return CreatedAtAction(nameof(GetComment), new {commentId = newComment.Id}, newComment);
    }

    [HttpPut("{commentId:guid}")]
    [Authorize]
    public async Task<IActionResult> PutComment(Guid commentId, CommentUpdateDto commentUpdateDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var role = User.FindFirstValue(ClaimTypes.Role);

        var comment = await _commentService.GetByIdAsync(commentId);

        if (comment == null) return NotFound();
        if (!comment.UserId.Equals(userId) && role != "Administrator")
            return Unauthorized(new {error_message = "The comment does not belong to this user"});

        await _commentService.UpdateAsync(commentId, commentUpdateDto);

        return NoContent();
    }

    [HttpDelete("{commentId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var role = User.FindFirstValue(ClaimTypes.Role);

        var comment = await _commentService.GetByIdAsync(commentId);

        if (comment == null) return NotFound();
        if (!comment.UserId.Equals(userId) && role != "Administrator")
            return Unauthorized(new {error_message = "The comment does not belong to this user"});

        await _commentService.DeleteAsync(commentId);

        return NoContent();
    }
}