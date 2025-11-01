namespace RabbitMQManipulation.Interfaces.Unbinders
{
    internal interface ISyncUnbinder
    {
        bool UnbindRabbitMQQueueAsync();
        bool DeleteRabbitMQQueueAsync();
    }
}
