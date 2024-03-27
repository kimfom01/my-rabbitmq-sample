using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ConsoleConsumer;

ConnectionFactory factory = new()
{
    // TODO: Use configuration to fetch uri from appsettings.json
    Uri = new Uri("amqp://user:password@rabbitmq:5672"),
    ClientProvidedName = "Rabbit Receiver 1 App"
};

using var connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

const string exchangeName = "DemoExchange";
const string routingKey = "demo-routing-key";
const string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);
channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (_, args) =>
{
    await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(5))); // Simulating real world constraints

    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    var queueMessage = JsonSerializer.Deserialize<QueueMessage>(message);

    Console.WriteLine($"Message Received: {queueMessage?.Message}");

    channel.BasicAck(args.DeliveryTag, false);
};

var consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
connection.Close();