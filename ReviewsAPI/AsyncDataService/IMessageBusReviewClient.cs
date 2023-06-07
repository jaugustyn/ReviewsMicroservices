using Infrastructure.AsyncDataServices.Dto;

namespace ReviewsAPI.AsyncDataService;

public interface IMessageBusReviewClient
{
    void PublishReviewDeleteEvent(ReviewDeletedPublisherDto dto);
}