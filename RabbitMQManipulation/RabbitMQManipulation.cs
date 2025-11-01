using RabbitMQ.Client;
using RabbitMQManipulation.Interfaces.Binders;
using RabbitMQManipulation.Interfaces.Unbinders;
using RabbitMQManipulation.Interfaces.Utilizers;
using RabbitMQManipulation.Structs;
using Factory = RabbitMQManipulation.RabbitMQFactory.RabbitMQFactory;

namespace RabbitMQManipulation
{
    internal class RabbitMQManipulationAsync
    {
        protected ConnectionFactory _factory;
        private RabbitMQResources _rabbitMQResources;
        public bool IsConnected { get; private set; } = false;
        private RabbitMQConfig _rabbitMQConfig;
        protected bool _disposed;
        private IAsyncBinder? _binderManager;
        private IAsyncUnbinder? _unbinderManager;
        private IAsyncUtilizer? _utilizerManager;

        public RabbitMQManipulationAsync(RabbitMQConfig rabbitMQConfig, RabbitMQResources rabbitMQResources)
        {
            _factory = new ConnectionFactory();
            _rabbitMQResources = rabbitMQResources;
            _rabbitMQConfig = rabbitMQConfig;

            // Enable automatic recovery
            _factory.AutomaticRecoveryEnabled = true;

            // attempt recovery every 5 seconds
            _factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);

            // Setup connection parameters
            _factory.Uri = new Uri(_rabbitMQConfig.Url);
            _rabbitMQResources = rabbitMQResources;
        }

        #region public methods
        public async Task<bool> ConnectAsync()
        {
            try
            {
                // Create connection 
                _rabbitMQResources.Connection = await _factory.CreateConnectionAsync();
                // Open channel
                _rabbitMQResources.Channel = await _rabbitMQResources.Connection.CreateChannelAsync();

                _binderManager = Factory.createAsyncBinderManager(_rabbitMQResources, _rabbitMQConfig);
                _unbinderManager = Factory.createAsyncUnbinderManager(_rabbitMQResources, _rabbitMQConfig);
                _utilizerManager = Factory.createAsyncUtilizerManager(_rabbitMQResources);

                IsConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                return false;
            }
        }

        // Binder
        public async Task<bool> CreateExchangeAsync()
        {
            if (!IsConnected) return false;
            return await _binderManager!.CreateExchangeAsync();
        }

        public async Task<bool> CreateQueueAsync()
        {
            if (!IsConnected) return false;
            return await _binderManager!.CreateQueueAsync();
        }

        public async Task<bool> BindRabbitMQQueueAsync()
        {
            if (!IsConnected) return false;
            return await _binderManager!.BindRabbitMQQueueAsync();
        }

        // Unbinder methods
        public async Task<bool> UnbindRabbitMQQueueAsync()
        {
            if (!IsConnected) return false;
            return await _unbinderManager!.UnbindRabbitMQQueueAsync();
        }

        public async Task<bool> DeleteRabbitMQQueueAsync()
        {
            if (!IsConnected) return false;
            return await _unbinderManager!.DeleteRabbitMQQueueAsync();
        }

        #endregion

        #region private methods

        // Utilizer
        private async Task CloseRabbitMQChannelAsync()
        {
            if (!IsConnected)
                throw new InvalidOperationException("You are not connected!");
            await _utilizerManager!.CloseChannelAsync();
        }

        private async Task CloseRabbitMQConnectionAsync()
        {
            if (!IsConnected)
                throw new InvalidOperationException("You are not connected!");
            await _utilizerManager!.CloseConnectionAsync();
        }

        private async Task DisposeRabbitMQChannelAsync()
        {
            if (!IsConnected)
                throw new InvalidOperationException("You are not connected!");
            await _utilizerManager!.DisposeChannelAsync();
        }

        private async Task DisposeRabbitMQConnectionAsync()
        {
            if (!IsConnected)
                throw new InvalidOperationException("You are not connected!");
            await _utilizerManager!.DisposeConnectionAsync();
        }
        #endregion

        ~RabbitMQManipulationAsync()
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

        virtual public void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            { /* free managed resources if any */  }

            if (!IsConnected)
                throw new InvalidOperationException("You are not connected!");
            // free unmanaged resources
            _unbinderManager!.UnbindRabbitMQQueueAsync();
            _unbinderManager!.DeleteRabbitMQQueueAsync();
            _rabbitMQResources.Channel?.Dispose();
            _rabbitMQResources.Connection?.Dispose();

            _rabbitMQResources.Channel = null;
            _rabbitMQResources.Connection = null;

            _disposed = true;
            IsConnected = false;
        }
    }
}
