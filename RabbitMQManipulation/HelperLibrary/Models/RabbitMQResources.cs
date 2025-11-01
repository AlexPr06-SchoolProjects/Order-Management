using RabbitMQ.Client;

namespace RabbitMQManipulation.HelperLibrary
{
    public class RabbitMQResources
    {
        public IConnection? Connection { get; set; }
        public IChannel? Channel { get; set; }

        public RabbitMQResources(IConnection? connection, IChannel? channel)
        {
            Connection = connection;
            Channel = channel;
        }
    }
}
