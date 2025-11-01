namespace RabbitMQManipulation.Interfaces.Binders
{
    internal interface ISyncBinder
    {
        bool CreateExchangeAsync();
        bool CreateQueueAsync();
        bool BindRabbitMQQueueAsync();
    }
}
