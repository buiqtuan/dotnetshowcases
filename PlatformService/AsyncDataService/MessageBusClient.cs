using PlatformService.Dtos;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace PlatformService.AsyncDataService;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;

    private readonly IConnection _connection;

    private readonly IModel _model;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        var factory = new ConnectionFactory() 
        { 
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"])
        };

        try
        {
            _connection = factory.CreateConnection();

            _model = _connection.CreateModel();

            _model.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdown +=  RabbitMQConnectionShutdown!;

            Console.WriteLine("--> Connected to MessageBus");
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not connection to message bus with error {e.Message}");
        }
    }

    public void PublishNewPlatform(PlatformPublishDto platform)
    {
        var message = JsonSerializer.Serialize(platform);

        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ connection is open, sending message ...");

            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ connection is closed...");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        
        _model.BasicPublish
        (
            exchange: "trigger",
            routingKey: "",
            basicProperties: null,
            body: body
        );

        Console.WriteLine($"--> Message has been sent {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("--> Message bus Desposed!");

        if (_model.IsOpen)
        {
            _model.Close();
            _connection.Close();
        }
    }

    private void RabbitMQConnectionShutdown(object sender, ShutdownEventArgs args)
    {
        Console.WriteLine("--> Rabbit MQ connection shut down!");
    }
    
}