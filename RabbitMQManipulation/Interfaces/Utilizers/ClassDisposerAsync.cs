namespace RabbitMQManipulation.Interfaces.Utilizers
{
    internal interface ClassDisposerAsync : IDisposable
    {
        ValueTask DisposeAsync();
        void Dispose(bool disposing);
    }
}
