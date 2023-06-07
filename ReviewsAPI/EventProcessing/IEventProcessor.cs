namespace ReviewsAPI.EventProcessing;

public interface IEventProcessor
{
    void ProcessEvent(string message);
}