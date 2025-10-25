using RabbitMQRpcConsumer = Consumer.Consumer;
using RabbitMQConfiguration = RabbitMQConfig.RabbitMQConfig;

// Set console output to UTF-8 to support emojis
Console.OutputEncoding = System.Text.Encoding.UTF8;

RabbitMQConfiguration rabbitMQConfig = new RabbitMQConfiguration();

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("👨‍🍳 The chef walks into the kitchen and checks the equipment...");
Console.ResetColor();

await using RabbitMQRpcConsumer consumer = new RabbitMQRpcConsumer(rabbitMQConfig);
bool isConnectedConsumer = await consumer.ConnectAsync();
if (!isConnectedConsumer)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ The chef couldn’t connect to RabbitMQ! Without communication with the waiters, no orders can come in!");
    Console.ResetColor();
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ Connection to RabbitMQ established! The waiters are already running with new orders!");
Console.ResetColor();

bool isQueueCreatedConsumer = await consumer.CreateQueueAsync();
if (!isQueueCreatedConsumer)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("🍽️ Failed to create the order queue! The dishes are hanging in the air 😅");
    Console.ResetColor();
    return;
}

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("🧾 The order queue is ready! The kitchen is waiting for the first customer order...");
Console.ResetColor();

bool isBoundConsumer = await consumer.BindRabbitMQQueueAsync();
if (!isBoundConsumer)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("🚫 Failed to bind the queue to the exchange! The orders are getting lost between the waiters!");
    Console.ResetColor();
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("👨‍🍳 The chef is ready to accept orders! 🔪🔥");
Console.ResetColor();

Console.ForegroundColor = ConsoleColor.DarkYellow;
Console.WriteLine("🍲 Waiting for customer orders... Press Ctrl + C to close the restaurant 😄");
Console.ResetColor();

await consumer.StartListeneningAsync();
