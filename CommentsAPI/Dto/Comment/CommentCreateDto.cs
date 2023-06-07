namespace CommentsAPI.Dto.Comment;

public class CommentCreateDto
{
    public Guid ReviewId { get; init; }
    public string Text { get; init; }
}