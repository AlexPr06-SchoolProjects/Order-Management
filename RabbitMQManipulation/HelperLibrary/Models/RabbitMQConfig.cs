using RabbitMQ.Client;

namespace RabbitMQManipulation.HelperLibrary
{
    public class Exchange
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Durable { get; set; }
        public Exchange(string name, string type, bool durable)
        {
            Name = name;
            Type = type;
            Durable = durable;
        }
    }

    public class Queue
    {
        public string Name { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }

        public Queue(string name, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            Name = name;
            Durable = durable;
            Exclusive = exclusive;
            AutoDelete = autoDelete;
            Arguments = arguments;
        }
    }

    public class RabbitMQConfig
    {
        public string User { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Url { get; set; }
        public string VirtualHost { get; set; }

        public Exchange Exchange { get; set; }
        public Queue Queue { get; set; }

        public RabbitMQConfig()
        {
            User = "amtuviim";
            Password = "3EWeg6-o-iw14BH-rsZ8c-fnygmpCB5K";
            Port = 5671;
            VirtualHost = "amtuviim";
            Url = "amqps://amtuviim:3EWeg6-o-iw14BH-rsZ8c-fnygmpCB5K@hawk.rmq.cloudamqp.com/amtuviim";
            Exchange = new Exchange("orders_exchange", ExchangeType.Direct, true);
            Queue = new Queue(
                name: "orders_queue", 
                durable: true,
                exclusive: false, 
                autoDelete: false, 
                arguments: new Dictionary<string, object>()
            );
        }
    }
}
