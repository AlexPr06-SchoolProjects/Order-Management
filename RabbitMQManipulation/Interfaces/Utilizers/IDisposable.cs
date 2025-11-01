namespace RabbitMQManipulation.Interfaces.Utilizers
{

    internal interface IRabbitMQSyncDisposable
    {
        void DisposeChannel();
        void DisposeConnection();
    }

    internal interface IRabbitMQAsyncDisposable
    {
        Task DisposeChannelAsync();
        Task DisposeConnectionAsync();
    }
}
