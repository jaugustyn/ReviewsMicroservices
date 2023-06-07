using Infrastructure.AsyncDataServices;
using ReviewsAPI.EventProcessing;

namespace ReviewsAPI.AsyncDataService;

public class MessageBusReviewSubscriber : MessageBusSubscriber
{
    private readonly IEventProcessor _eventProcessor;

    public MessageBusReviewSubscriber(IEventProcessor eventProcessor)
    {
        _eventProcessor = eventProcessor;
    }

    protected override void ProcessEvent(string message)
    {
        _eventProcessor.ProcessEvent(message);
    }
}