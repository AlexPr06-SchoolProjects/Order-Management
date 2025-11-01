using RabbitMQManipulation.Interfaces.Binders;

namespace RabbitMQManipulation.HelperLibrary.Async
{
    internal class AsyncBinderManager : IAsyncBinder
    {
        private RabbitMQResources _rabbitMQResources;
        private RabbitMQConfig _rabbitMQConfig;

        public AsyncBinderManager (RabbitMQResources rabbitMQResources, RabbitMQConfig rabbitMQConfig)
        {
            _rabbitMQResources = rabbitMQResources;
            _rabbitMQConfig = rabbitMQConfig;
        }
        public async Task<bool> CreateExchangeAsync()
        {
            if (_rabbitMQResources.Channel is null)
                return false;

            try
            {
                await _rabbitMQResources.Channel.ExchangeDeclareAsync(
                    exchange: _rabbitMQConfig.Exchange.Name,
                    type: _rabbitMQConfig.Exchange.Type,
                    durable: false,
                    autoDelete: false,
                    arguments: null,
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
        public async Task<bool> CreateQueueAsync() {
            return await Task.FromResult( false );
        }
        public async Task<bool> BindRabbitMQQueueAsync() {
            if (_rabbitMQResources.Channel is null)
                return false;
            try
            {
                await _rabbitMQResources.Channel.QueueBindAsync(
                   queue: _rabbitMQConfig.Queue.Name,
                   exchange: _rabbitMQConfig.Exchange.Name,
                   routingKey: _rabbitMQConfig.Queue.Name,
                   arguments: null,
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
    }
}
