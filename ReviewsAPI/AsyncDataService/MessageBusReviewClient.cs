using System.Text.Json;
using Infrastructure.AsyncDataServices;
using Infrastructure.AsyncDataServices.Dto;

namespace ReviewsAPI.AsyncDataService;

public class MessageBusReviewClient : MessageBusClient, IMessageBusReviewClient
{
    public MessageBusReviewClient(IConfiguration configuration) : base(configuration)
    {
    }

    public void PublishReviewDeleteEvent(ReviewDeletedPublisherDto dto)
    {
        var message = JsonSerializer.Serialize(dto);

        if (Connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ connection open, sending message...");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ connection closed, not sending");
        }
    }
}