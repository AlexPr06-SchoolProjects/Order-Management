
using RabbitMQ.Client;
using Order_Management.RabbitMQConfig;

namespace RabbitMQManipulation
{
    abstract public class RabbitMQManipulation : IAsyncDisposable
    {
        protected ConnectionFactory _factory;
        protected IConnection? _connection;
        protected IChannel? _channel;
        protected bool _disposed;
        public  RabbitMQConfig Config { get; set; }

        public RabbitMQManipulation(RabbitMQConfig config)
        {
            _factory = new ConnectionFactory();
            Config = config;

            // Enable automatic recovery
            _factory.AutomaticRecoveryEnabled = true;

            // attempt recovery every 10 seconds
            _factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);

            // Setup connection parameters
            _factory.Uri = new Uri(config.Url);

            //_factory.ClientProvidedName = "app:order-management component:event-consumer";
        }

        ~RabbitMQManipulation() 
        {
            Dispose(false);
        }

        virtual public async ValueTask DisposeAsync()
        {
            await UnbindRabbitMQQueueAsync();
            await DeleteRabbitMQQueueAsync();
            await CloseRabbitMQChannelAsync();
            await CloseRabbitMQConnectionAsync();
            await DisposeRabbitMQChannelAsync();
            await DisposeRabbitMQConnectionAsync();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        #region public methods
        public async Task<bool> ConnectAsync()
        {
            try
            {
                // Create connection 
                _connection = await _factory.CreateConnectionAsync();
                // Open channel
                _channel = await _connection.CreateChannelAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateExchangeAsync()
        {
            if (_channel is null)
                return false;

            try
            {
                await _channel.ExchangeDeclareAsync(
                    exchange: Config.Exchange.Name,
                    type: Config.Exchange.Type,
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

        abstract public Task<bool> CreateQueueAsync();

        public async Task<bool> BindRabbitMQQueueAsync()
        {
            if (_channel is null)
                return false;
            try
            {
                await _channel.QueueBindAsync(
                   queue: Config.Queue.Name,
                   exchange: Config.Exchange.Name,
                   routingKey: Config.Queue.Name,
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

        public async Task<bool> UnbindRabbitMQQueueAsync()
        {
            if (_channel is null)
                return false;
            try
            {
                await _channel.QueueUnbindAsync(
                    queue: Config.Queue.Name,
                    exchange: Config.Exchange.Name,
                    routingKey: Config.Queue.Name,
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

        public async Task<bool> DeleteRabbitMQQueueAsync()
        {
            if (_channel is null)
                return false;
            try
            {
                await _channel.QueueDeleteAsync(
                     queue: Config.Queue.Name,
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

        public byte[] ConvertMessageToBytes(string message) => System.Text.Encoding.UTF8.GetBytes(message);
       
        #endregion

        #region private methods
        virtual protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            { /* free managed resources if any */  }


            // free unmanaged resources

            try
            {
                _channel?.QueueUnbindAsync(
                   queue: Config.Queue.Name,
                   exchange: Config.Exchange.Name,
                   routingKey: Config.Queue.Name,
                   arguments: null,
                   cancellationToken: default
                );
                _channel?.QueueDeleteAsync(
                     queue: Config.Queue.Name,
                     ifUnused: false,
                     ifEmpty: false,
                     noWait: false,
                     cancellationToken: default
                );
                _channel?.Dispose();
                _connection?.Dispose();
            }
            catch { /* ignore */}

            _channel = null;
            _connection = null;

            _disposed = true;
        }

        private async Task CloseRabbitMQChannelAsync()
        {
            if (_channel is null || _channel.IsClosed)
                return;
            await _channel.CloseAsync();
            _channel = null;
        }

        private async Task CloseRabbitMQConnectionAsync()
        {
            if (_connection is null || !_connection.IsOpen)
                return;
            await _connection.CloseAsync();
            _connection = null;
        }

        private async Task DisposeRabbitMQChannelAsync()
        {
            if (_channel is null)
                return;
            await _channel.DisposeAsync();
            _channel = null;
        }

        private async Task DisposeRabbitMQConnectionAsync()
        {
            if (_connection is null)
                return;
            await _connection.DisposeAsync();
            _connection = null;
        }
        #endregion
    }
}
