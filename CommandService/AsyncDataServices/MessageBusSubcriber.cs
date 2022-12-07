using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandService.AsyncDataServices;

public class MessageBusSubcriber : BackgroundService
{
    private readonly IConfiguration _configuration;

    private readonly IEventProcessor _eventProcessor;

    private IConnection? _connection;

    private IModel? _model;

    public string? _queueName;

    public MessageBusSubcriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"]),
        };

        _connection = factory.CreateConnection();
        _model = _connection.CreateModel();
        _model.ExchangeDeclare
        (
            exchange: "trigger", 
            type: ExchangeType.Fanout
        );

        _queueName = _model.QueueDeclare().QueueName;
        _model.QueueBind
        (
            queue: _queueName, 
            exchange: "trigger", 
            routingKey: ""
        );

        Console.WriteLine("--> Listenning on the message buss...");

        _connection.ConnectionShutdown +=  RabbitMQConnectionShutdown!;
    }

    private void RabbitMQConnectionShutdown(object sender, ShutdownEventArgs args)
    {
        Console.WriteLine("--> Connection to Message bus shuting down...");
    }

    public override void Dispose()
    {
        if (_model!.IsOpen)
        {
            _connection!.Close();
            _model.Close();
        }

        base.Dispose();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_model);

        consumer.Received += (ModuleHandle, ea) =>
        {
            Console.WriteLine("--> Event Received!");

            var body = ea.Body;

            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            _eventProcessor.ProcessEvent(notificationMessage);
        };

        _model.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}