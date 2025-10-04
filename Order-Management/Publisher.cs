using Order_Management.RabbitMQConfig;
using RabbitMQClass = RabbitMQManipulation.RabbitMQManipulation;

RabbitMQConfig rabbitMQConfig = new RabbitMQConfig();

await using RabbitMQClass rabbitMQ = new RabbitMQClass(rabbitMQConfig);
bool isConnected = await rabbitMQ.ConnectAsync();
if (!isConnected)
{
    Console.WriteLine("Failed to connect to RabbitMQ.");
    return;
}

bool isExchangeCreated = await rabbitMQ.CreateExchangeAsync();
if (!isExchangeCreated)
{
    Console.WriteLine("Failed to create exchange.");
    return;
}

Console.WriteLine("Exchange created successfully.");

bool isQueueCreated = await rabbitMQ.CreateQueueAsync();

if (!isQueueCreated)
{
    Console.WriteLine("Failed to create queue.");
    return;
}
Console.WriteLine("Queue created successfully.");

bool isBound = await rabbitMQ.BindRabbitMQQueueAsync();
if (!isBound)
{
    Console.WriteLine("Failed to bind queue to exchange.");
    return;
}

string message = "Order Created: OrderID 12345";

bool isPublished = await rabbitMQ.PublishMessageAsync(message);
if (!isPublished)
{
    Console.WriteLine("Failed to publish message.");
    return;
}
Console.WriteLine("Message published successfully.");



