namespace Infrastructure.AsyncDataServices.Dto;

public class UserDeletedPublisherDto
{
    public Guid UserId { get; set; }
    public string Event { get; set; }

    public UserDeletedPublisherDto() {}
    public UserDeletedPublisherDto(Guid userId, string eventName)
    {
        UserId = userId;
        Event = eventName;
    }
}