using RabbitMQ.Client;

namespace RabbitMQManipulation.Structs
{
    internal struct RabbitMQResources
    {
        public IConnection? Connection { get; set; }
        public IChannel? Channel { get; set; }

        public RabbitMQResources(IConnection connection, IChannel channel)
        {
            Connection = connection;
            Channel = channel;
        }
    }
}
