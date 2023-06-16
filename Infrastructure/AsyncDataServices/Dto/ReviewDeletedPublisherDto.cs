namespace Infrastructure.AsyncDataServices.Dto;

public class ReviewDeletedPublisherDto
{
    public Guid ReviewId { get; set; }
    public string Event { get; set; }

    public ReviewDeletedPublisherDto(){ }
    public ReviewDeletedPublisherDto(Guid reviewId, string eventName)
    {
        ReviewId = reviewId;
        Event = eventName;
    }
}