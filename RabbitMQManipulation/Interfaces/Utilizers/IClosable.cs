namespace RabbitMQManipulation.Interfaces.Utilizers
{
    internal interface IRabbitMQSyncClosable
    {
        void CloseChannel();
        void CloseConnection();
    }

    internal interface IRabbitMQAsyncClosable
    {
        Task CloseChannelAsync();
        Task CloseConnectionAsync();
    }
}
