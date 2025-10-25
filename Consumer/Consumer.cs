namespace Consumer
{
    using Newtonsoft.Json.Linq;
    using GUIManager;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQConfig;
    using System.Text;
    using System.Threading.Tasks;
    using RabbitMQClass = RabbitMQManipulation.RabbitMQManipulation;
 
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


            var consumer = new AsyncEventingBasicConsumer(_channel!);

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

                    consumer.ReceivedAsync += async (model, ea) =>
                    {
                        string response = string.Empty;
                        try
                        {
                            var body = ea.Body.ToArray();
                            string jsonMessage = Encoding.UTF8.GetString(body);
                            object message = JsonConvert.DeserializeObject<object>(jsonMessage)!;

                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"🍽️  New order received from a customer:");
                            Console.ResetColor();
                            gUIManager.DisplayOrderInTable(jsonMessage);

                            response = ProcessRequestAsync(jsonMessage);

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

        private string ProcessRequestAsync(string request)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("🔪 Preparing the dish...");
            Console.ResetColor();

            try
            {
                // Parse the incoming JSON order
                var order = JObject.Parse(request);

                int orderId = (int)order["OrderId"]!;
                int amount = (int)order["Amount"]!;
                string dishName = (string)order["OrderedDish"]!["Name"]!;
                double price = (double)order["OrderedDish"]!["Price"]!;

                // Simulate cooking progress
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"⏳ Cooking {amount}x {dishName} (Order #{orderId})...");
                Console.ResetColor();


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"🍽️ {dishName} is ready! Served for ${price * amount:F2} 💵");
                Console.ResetColor();

                // Return confirmation
                var response = new JObject
                {
                    ["OrderId"] = orderId,
                    ["Status"] = "Ready",
                    ["Dish"] = dishName,
                    ["Amount"] = amount,
                    ["Price"] = price,
                };

                return response.ToString();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Failed to process order: {ex.Message}");
                Console.ResetColor();
                return JsonConvert.SerializeObject(new { Status = "Error", Message = ex.Message });
            }
        }  
    }
}


