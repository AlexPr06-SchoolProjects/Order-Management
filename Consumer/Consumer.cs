namespace Consumer
{
    using RabbitMQConfig;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.Text;
    using System.Threading.Tasks;
    using RabbitMQClass = RabbitMQManipulation.RabbitMQManipulation;
    using GUIManager;
    using Newtonsoft.Json;
 
    public class Consumer : RabbitMQClass
    {
        private GUIManager gUIManager;
        public Consumer(RabbitMQConfig config) : base(config)
        {
            gUIManager = new GUIManager();
        }

        override public async Task<bool> CreateQueueAsync()
        {
            if (_channel is null)
                return false;
            try
            {
                await _channel.QueueDeclareAsync(
                    queue: Config.Queue.Name,
                    durable: Config.Queue.Durable,
                    exclusive: Config.Queue.Exclusive,
                    autoDelete: Config.Queue.AutoDelete,
                    arguments: null,
                    passive: false,
                    noWait: false,
                    cancellationToken: default
                );
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task StartListeneningAsync()
{
    if (_channel is null)
        throw new Exception("Channel is not initialized.");


    while (true)
    {
        try
        {
            // if connection lost — try to reconnect
            if (_channel is null || !_channel.IsOpen)
            {

                bool reconnected = await ConnectAsync();
                if (!reconnected)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Failed to reconnect. Trying again in 5 seconds...");
                    Console.ResetColor();
                    await Task.Delay(5000);
                    continue;
                }

                await CreateQueueAsync();
                await BindRabbitMQQueueAsync();
            }

            var consumer = new AsyncEventingBasicConsumer(_channel!);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                string response = string.Empty;
                try
                {
                    var body = ea.Body.ToArray();
                    var jsonMessage = Encoding.UTF8.GetString(body);
                    object message = JsonConvert.DeserializeObject<object>(jsonMessage)!;

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"🍽️  New order received from a customer:");
                    Console.ResetColor();
                    gUIManager.DisplayOrderInTable(jsonMessage);

                    response = await ProcessRequestAsync(message);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✅ Order completed successfully!");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Error while preparing the dish: {ex.Message}");
                    Console.ResetColor();
                    response = $"Error: {ex.Message}";
                }
                finally
                {
                    // Send response back
                    var replyProps = new BasicProperties
                    {
                        CorrelationId = ea.BasicProperties?.CorrelationId
                    };
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    var replyTo = ea.BasicProperties?.ReplyTo;

                    if (!string.IsNullOrEmpty(replyTo))
                    {
                        await _channel!.BasicPublishAsync(
                            exchange: "",
                            routingKey: replyTo!,
                            mandatory: false,
                            basicProperties: replyProps,
                            body: responseBytes
                        );
                    }

                    await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }

                await Task.Yield();
            };

            // Start consuming messages
            await _channel!.BasicConsumeAsync(
                queue: Config.Queue.Name,
                autoAck: false,
                consumer: consumer,
                cancellationToken: default
            );

            // Keep the kitchen alive indefinitely
            await Task.Delay(Timeout.Infinite);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"🔥 Kitchen error: {ex.Message}. Reconnecting in 5 seconds...");
            Console.ResetColor();
            await Task.Delay(5000);
        }
    }
}



        private async Task<string> ProcessRequestAsync(object request)
            {
            await Task.Delay(500);
            var result = new string(request.ToString());

            return result;
        }
    }
}


