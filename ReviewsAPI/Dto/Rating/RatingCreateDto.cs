namespace ReviewsAPI.Dto.Rating;

public class RatingCreateDto
{
    public Guid ReviewId {get; set;}
    public int Value { get; init; }
}