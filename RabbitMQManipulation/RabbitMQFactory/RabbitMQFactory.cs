using RabbitMQManipulation.HelperLibrary.Async;
using RabbitMQManipulation.HelperLibrary.Structs;
using RabbitMQManipulation.Interfaces.Binders;
using RabbitMQManipulation.Interfaces.Unbinders;
using RabbitMQManipulation.Interfaces.Utilizers;

namespace RabbitMQManipulation.RabbitMQFactory
{
    internal static class RabbitMQFactory
    {
        public static RabbitMQResources createRabbitMQResources() => new RabbitMQResources();
        public static RabbitMQConfig createRabbitMQConfig() => new RabbitMQConfig();
        public static IAsyncBinder createAsyncBinderManager(RabbitMQResources rabbitMQResources, RabbitMQConfig rabbitMQConfig) => new AsyncBinderManager(rabbitMQResources, rabbitMQConfig);
        public static IAsyncUnbinder createAsyncUnbinderManager(RabbitMQResources rabbitMQResources, RabbitMQConfig rabbitMQConfig) => new AsyncUnbinderManager(rabbitMQResources, rabbitMQConfig);
        public static IAsyncUtilizer createAsyncUtilizerManager(RabbitMQResources rabbitMQResources) => new AsyncUtilizerManager(rabbitMQResources);
    }
}
