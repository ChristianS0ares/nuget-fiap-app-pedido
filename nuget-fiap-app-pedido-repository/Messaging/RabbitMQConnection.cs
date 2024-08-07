using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Serialization.SystemTextJson;

namespace nuget_fiap_app_pedido_repository.Messaging
{
    public sealed class RabbitMQConnection
    {
        public IBus bus;
        public RabbitMQConnection() 
        {
            var connectionString = Environment.GetEnvironmentVariable("HOST_RABBITMQ") ?? "host=localhost";
            bus = RabbitHutch.CreateBus(connectionString, x => x.Register<ISerializer, SystemTextJsonSerializer>());
        }

    }
}
