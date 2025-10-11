namespace Consumer
{
    using Order_Management.RabbitMQConfig;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.Text;
    using System.Threading.Tasks;
    using RabbitMQClass = RabbitMQManipulation.RabbitMQManipulation;
    public class Consumer : RabbitMQClass
    {
        public Consumer(RabbitMQConfig config) : base(config) {}

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

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                string response = string.Empty;
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    response = await ProcessRequestAsync(message);
                }
                catch (Exception ex)
                {
                    response = $"Error: {ex.Message}";
                }
                finally
                {
                    // Send response back to the reply queue
                    var replyProps = new BasicProperties
                    {
                        CorrelationId = ea.BasicProperties?.CorrelationId
                    };
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                   
                    var replyTo = ea.BasicProperties?.ReplyTo;
                    if (!string.IsNullOrEmpty(replyTo))
                    {
                       await _channel.BasicPublishAsync(
                           exchange: "",
                           routingKey: ea.BasicProperties?.ReplyTo,
                           mandatory: false,
                           basicProperties: replyProps,
                           body: responseBytes
                       );
                    }
                 
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                }
                await Task.Yield();
            };

            // Start consuming messages
            await _channel.BasicConsumeAsync(
                queue: Config.Queue.Name,
                autoAck: false,
                consumer: consumer,
                cancellationToken: default
            );
        }

        private async Task<string> ProcessRequestAsync(string request)
            {
                await Task.Delay(500); // Simulate processing time
            var result = new string(request.Reverse().ToArray());
                return result;
        }
    }
}


