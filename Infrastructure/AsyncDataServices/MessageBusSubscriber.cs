using System.Text;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.AsyncDataServices;

public abstract class MessageBusSubscriber : BackgroundService
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly string _queueName;

    protected MessageBusSubscriber()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@rabbitmq:5672/"),
            AutomaticRecoveryEnabled = true,
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("trigger", ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(_queueName, "trigger", "");

            Console.WriteLine("--> Listening on the Message Bus...");
        }
        catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
        {
            Console.WriteLine($"--> Connection failed: {ex.Message}, retrying in 2s...");
            Thread.Sleep(2000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Connection failed: {ex.Message}");
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (moduleHandle, ea) =>
        {
            Console.WriteLine("--> Event Received!");
            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            ProcessEvent(notificationMessage);
        };

        _channel.BasicConsume(_queueName, true, consumer);

        return Task.CompletedTask;
    }

    protected abstract void ProcessEvent(string message);

    public override void Dispose()
    {
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }

        base.Dispose();
    }
}