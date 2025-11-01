namespace Publisher
{
    using RabbitMQConfig;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.Collections.Concurrent;
    using System.Text;
    using RabbitMQClass = RabbitMQManipulation.RabbitMQManipulation;
    using Newtonsoft.Json;

    public class Publisher : RabbitMQClass
    {
        private string _replyQueueName;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingResponses = new();
        public Publisher(RabbitMQConfig config) : base(config)
        {
            _replyQueueName = string.Empty;
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

        public async Task<bool> InitializeRpcAsync()
        {
            if (_channel is null)
                throw new InvalidOperationException("Channel is not initialized.");

            // Temporary queue for answers
            var replyQueue = await _channel.QueueDeclareAsync(
                    queue: "",
                    durable: false,
                    exclusive: true,
                    autoDelete: true,
                    arguments: null,
                    passive: false,
                    noWait: false,
                    cancellationToken: default
                );
            _replyQueueName = replyQueue.QueueName;

            // Creating consumer for answers
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                if (ea.BasicProperties?.CorrelationId is not null && _pendingResponses.TryRemove(ea.BasicProperties.CorrelationId, out var tcs))
                {
                    var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                    tcs.TrySetResult(response);
                }
                await Task.Yield();
            };

            await _channel.BasicConsumeAsync(
                queue: _replyQueueName,
                autoAck: true,
                consumer: consumer,
                cancellationToken: default
            );

            return true;
        }

        public async Task<string> CallRpcAsync(object message, TimeSpan timeout)
        {
            if (_channel is null)
                throw new InvalidOperationException("Channel is not initialized.");

            string correlationId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            _pendingResponses[correlationId] = tcs;

            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = _replyQueueName
            };

            string jsonMessage = JsonConvert.SerializeObject(message);

            await _channel.BasicPublishAsync(
                exchange: Config.Exchange.Name,
                routingKey: Config.Queue.Name,
                mandatory: false,
                basicProperties: props,
                body: Encoding.UTF8.GetBytes(jsonMessage)
            );
            using var cts = new CancellationTokenSource(timeout);

            cts.Token.Register(() =>
            {
                if (_pendingResponses.TryRemove(correlationId, out var removed))
                {
                    removed.TrySetCanceled();
                }
            });

            try
            {
                return await tcs.Task;
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException("The RPC call timed out.");
            }
            finally
            {
                _pendingResponses.TryRemove(correlationId, out _);
            }
        }

        public async Task DisposePublisherAsync()
        {
            // Cancel pending RPC calls
            foreach (var tcs in _pendingResponses.Values)
                tcs.TrySetCanceled();

            _pendingResponses.Clear();

            // Close channel if open
            if (_channel?.IsOpen == true)
                await _channel.CloseAsync();

            _channel?.Dispose();
        }
    }
}
