using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace RabbitMQAPI;

public class QueueService : RabbitMqClientBase
{
    public QueueService(
        ConnectionFactory factory,
        IConfiguration configuration) :
        base(factory, configuration)
    {
    }

    public async Task PublishMessage(QueueMessage queueMessage)
    {
        var message = JsonSerializer.Serialize(queueMessage);
        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
        await Task.Run(() => Channel.BasicPublish(ExchangeName, RoutingKey, null, messageBodyBytes));
    }
}