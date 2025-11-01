namespace RabbitMQManipulation.Interfaces.Binders
{
    internal interface IAsyncBinder
    {
        Task<bool> CreateExchangeAsync();
        Task<bool> CreateQueueAsync();
        Task<bool> BindRabbitMQQueueAsync();
    }
}
