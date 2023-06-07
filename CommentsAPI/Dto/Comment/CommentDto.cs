﻿using Core.Entities;

namespace CommentsAPI.Dto.Comment;

public class CommentDto : IEntityBase
{
    public string Text { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
    public Guid ReviewId { get; init; }
    public Guid UserId { get; init; }
    public Guid Id { get; init; }

    public static CommentDto CommentToDto(Core.Entities.Models.Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Text = comment.Text,
            CreatedDate = comment.CreatedDate,
            ReviewId = comment.ReviewId,
            UserId = comment.UserId
        };
    }
}