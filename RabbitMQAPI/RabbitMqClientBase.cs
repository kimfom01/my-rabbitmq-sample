using RabbitMQ.Client;

namespace RabbitMQAPI;

public class RabbitMqClientBase : IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    protected readonly IModel Channel;

    protected string ExchangeName { get; private set; }
    protected string RoutingKey { get; private set; }
    private string QueueName { get; set; }

    protected RabbitMqClientBase(
        ConnectionFactory factory,
        IConfiguration configuration)
    {
        _connection = factory.CreateConnection();
        Channel = _connection.CreateModel();
        _configuration = configuration;
        ConnectToRabbitMq();
    }

    private void ConnectToRabbitMq()
    {
        ExchangeName = _configuration.GetValue<string>("RabbitMQ:Exchange") ??
                       throw new NullReferenceException("RabbitMQ:URI not set or invalid");
        RoutingKey = _configuration.GetValue<string>("RabbitMQ:RoutingKey") ??
                     throw new NullReferenceException("RabbitMQ:RoutingKey not set or invalid");
        QueueName = _configuration.GetValue<string>("RabbitMQ:QueueName") ??
                    throw new NullReferenceException("RabbitMQ:QueueName not set or invalid");
        Channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
        Channel.QueueDeclare(QueueName, false, false, false, null);
        Channel.QueueBind(QueueName, ExchangeName, RoutingKey, null);
    }

    public void Dispose()
    {
        _connection.Dispose();
        Channel.Dispose();
    }
}