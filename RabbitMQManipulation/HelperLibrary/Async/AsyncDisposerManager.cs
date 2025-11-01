using RabbitMQManipulation.Interfaces.Unbinders;
using RabbitMQManipulation.Interfaces.Utilizers;

namespace RabbitMQManipulation.HelperLibrary.Async
{
    internal class AsyncDisposerManager : IAsyncDisposable
    {
        public bool IsDisposed { get; private set; }
        private object _classPtr;
        private IAsyncUnbinder? _unbinderManager;
        private IAsyncUtilizer? _utilizerManager;

        public AsyncDisposerManager(IAsyncUnbinder unbinderManager, IAsyncUtilizer utilizerManager, object classPtr) {
            _unbinderManager = unbinderManager;
            _utilizerManager = utilizerManager;
            _classPtr = classPtr;
        }

        virtual public async ValueTask DisposeAsync()
        {
            if (_unbinderManager == null || _utilizerManager == null)
                throw new Exception("UnbinderManager or UtilizerManager is not exist.");
            await _unbinderManager.UnbindRabbitMQQueueAsync();
            await _unbinderManager.DeleteRabbitMQQueueAsync();
            await _utilizerManager.CloseChannelAsync();
            await _utilizerManager.CloseConnectionAsync();
            await _utilizerManager.DisposeChannelAsync();
            await _utilizerManager.DisposeConnectionAsync();
            IsDisposed = true;
            GC.SuppressFinalize(_classPtr);
        }

        public async void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            { /* free managed resources if any */  }


            // free unmanaged resources

            try
            {
                bool queueUnbinded = _unbinderManager.UnbindRabbitMQQueueAsync();
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

            IsDisposed = true;
        }
    }
}
