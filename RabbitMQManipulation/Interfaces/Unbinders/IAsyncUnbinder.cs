namespace RabbitMQManipulation.Interfaces.Unbinders
{
    internal interface IAsyncUnbinder
    {
        Task<bool> UnbindRabbitMQQueueAsync();
        Task<bool> DeleteRabbitMQQueueAsync();
    }
}
