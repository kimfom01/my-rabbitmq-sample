using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQAPI;

var builder = WebApplication.CreateBuilder(args);

var user =
    builder.Configuration.GetValue<string>("RabbitMQ:User")
    ?? throw new NullReferenceException("RabbitMQ:URI not set or invalid");

var password =
    builder.Configuration.GetValue<string>("RabbitMQ:Password")
    ?? throw new NullReferenceException("RabbitMQ:URI not set or invalid");

var host =
    builder.Configuration.GetValue<string>("RabbitMQ:Host")
    ?? throw new NullReferenceException("RabbitMQ:URI not set or invalid");

var port =
    builder.Configuration.GetValue<string>("RabbitMQ:Port")
    ?? throw new NullReferenceException("RabbitMQ:URI not set or invalid");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(
    new ConnectionFactory
    {
        Uri = new Uri($"amqp://{user}:{password}@{host}:{port}"),
        ClientProvidedName = "Rabbit Sender App"
    }
);
builder.Services.AddTransient<QueueService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost(
        "/api/publish",
        async ([FromBody] QueueMessage message, [FromServices] QueueService queueService) =>
        {
            try
            {
                await queueService.PublishMessage(message);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex);
            }
        }
    )
    .WithOpenApi();

app.Run();