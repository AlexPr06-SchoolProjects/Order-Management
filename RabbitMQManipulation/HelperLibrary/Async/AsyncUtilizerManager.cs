using RabbitMQ.Client;
using RabbitMQManipulation.Interfaces.Utilizers;

namespace RabbitMQManipulation.HelperLibrary.Async
{
    internal class AsyncUtilizerManager : IAsyncUtilizer
    {
        private RabbitMQResources _rabbitMQResources;
        public AsyncUtilizerManager(RabbitMQResources rabbitMQResources)
        {
            _rabbitMQResources = rabbitMQResources;
        }

        public async Task CloseChannelAsync()
        {
            if (_rabbitMQResources.Channel is null || _rabbitMQResources.Channel.IsClosed)
                return;
            await _rabbitMQResources.Channel.CloseAsync();
            _rabbitMQResources.Channel = null;
        }

        public async Task CloseConnectionAsync()
        {
            if (_rabbitMQResources.Connection is null || !_rabbitMQResources.Connection.IsOpen)
                return;
            await _rabbitMQResources.Connection.CloseAsync();
            _rabbitMQResources.Connection = null;
        }

        public async Task DisposeChannelAsync()
        {
            if (_rabbitMQResources.Channel is null)
                return;
            await _rabbitMQResources.Channel.DisposeAsync();
            _rabbitMQResources.Channel = null;
        }

        public async Task DisposeConnectionAsync()
        {
            if (_rabbitMQResources.Connection is null)
                return;
            await _rabbitMQResources.Connection.DisposeAsync();
            _rabbitMQResources.Connection = null;
        }
    }
}
