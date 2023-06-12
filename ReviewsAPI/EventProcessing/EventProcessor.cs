using System.Text.Json;
using Core.Interfaces.Repositories;
using Infrastructure.AsyncDataServices.Dto;

namespace ReviewsAPI.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IReviewRepository _reviewRepository;

    public EventProcessor(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public void ProcessEvent(string notificationMessage)
    {
        var eventType = DetermineEvent(notificationMessage);

        switch (eventType)
        {
            case EventType.UserDeleted:
                UserDeleteEvent(notificationMessage);
                break;
            case EventType.Undetermined:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEvent>(notificationMessage);

        if (eventType is null) return EventType.Undetermined;

        switch (eventType.Event)
        {
            case "User_Deleted":
                Console.WriteLine("--> User Delete Event Detected");
                return EventType.UserDeleted;
            default:
                Console.WriteLine("--> Could not determine the event type");
                return EventType.Undetermined;
        }
    }

    private void UserDeleteEvent(string userPublishedMessage)
    {
        var userPublishedDto = JsonSerializer.Deserialize<UserDeletedPublisherDto>(userPublishedMessage);

        if (userPublishedDto is null) return;

        var commentsList = _reviewRepository.GetReviewsByUserIdSync(userPublishedDto.UserId);

        foreach (var comment in commentsList) _reviewRepository.DeleteAsync(comment.Id);
    }
}

internal enum EventType
{
    UserDeleted,
    Undetermined
}