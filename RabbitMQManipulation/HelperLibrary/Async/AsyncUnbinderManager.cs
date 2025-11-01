using RabbitMQManipulation.HelperLibrary.Structs;
using RabbitMQManipulation.Interfaces.Unbinders;

namespace RabbitMQManipulation.HelperLibrary.Async
{
    internal class AsyncUnbinderManager : IAsyncUnbinder
    {
        private RabbitMQResources _rabbitMQResources;
        private RabbitMQConfig _rabbitMQConfig;

        public AsyncUnbinderManager(RabbitMQResources rabbitMQResources, RabbitMQConfig rabbitMQConfig)
        {
            _rabbitMQResources = rabbitMQResources;
            _rabbitMQConfig = rabbitMQConfig;
        }

        public async Task<bool> UnbindRabbitMQQueueAsync() {
            if (_rabbitMQResources.Channel is null)
                return false;
            try
            {
                await _rabbitMQResources.Channel.QueueUnbindAsync(
                    queue: _rabbitMQConfig.Queue.Name,
                    exchange: _rabbitMQConfig.Exchange.Name,
                    routingKey: _rabbitMQConfig.Queue.Name,
                    arguments: null,
                    cancellationToken: default
                );
            }
            catch
            {
                return false;
            }
            return true;
        }
        public async Task<bool> DeleteRabbitMQQueueAsync() {
            if (_rabbitMQResources.Channel is null)
                return false;
            try
            {
                await _rabbitMQResources.Channel.QueueDeleteAsync(
                     queue: _rabbitMQConfig.Queue.Name,
                     ifUnused: false,
                     ifEmpty: false,
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
