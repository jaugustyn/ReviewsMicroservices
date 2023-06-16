using System.Text.Json;
using Core.Interfaces.Repositories;
using Infrastructure.AsyncDataServices.Dto;

namespace ReviewsAPI.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IRatingRepository _ratingRepository;

    public EventProcessor(IReviewRepository reviewRepository, IRatingRepository ratingRepository)
    {
        _reviewRepository = reviewRepository;
        _ratingRepository = ratingRepository;
    }

    public void ProcessEvent(string notificationMessage)
    {
        var eventType = DetermineEvent(notificationMessage);

        switch (eventType)
        {
            case EventType.ReviewDeleted:
                ReviewDeleteEvent(notificationMessage);
                break;
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
            case "Review_Deleted":
                Console.WriteLine("--> Review Delete Event Detected");
                return EventType.ReviewDeleted;
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

        var reviewsList = _reviewRepository.GetReviewsByUserIdSync(userPublishedDto.UserId);
        foreach (var review in reviewsList) _reviewRepository.DeleteAsync(review.Id);
        
        var ratingsList = _ratingRepository.GetRatingsByUserIdSync(userPublishedDto.UserId);
        foreach (var rating in ratingsList) _ratingRepository.DeleteAsync(rating.Id);
    }
    
    private void ReviewDeleteEvent(string reviewPublishedMessage)
    {
        var reviewPublishedDto = JsonSerializer.Deserialize<ReviewDeletedPublisherDto>(reviewPublishedMessage);

        Console.WriteLine("--> reviewPublishedDto check.");
        if (reviewPublishedDto is null) return;

        var ratingsList = _ratingRepository.GetRatingsByReviewIdSync(reviewPublishedDto.ReviewId);
        foreach (var rating in ratingsList) _ratingRepository.DeleteAsync(rating.Id);
        Console.WriteLine("--> Ratings deleted.");
    }
}

internal enum EventType
{
    ReviewDeleted,
    UserDeleted,
    Undetermined
}