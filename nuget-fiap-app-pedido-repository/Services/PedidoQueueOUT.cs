using EasyNetQ;
using nuget_fiap_app_pedido_common.Interfaces.Repository;
using nuget_fiap_app_pedido_repository.Messaging;

namespace nuget_fiap_app_pedido_repository.Services
{
    public class PedidoQueueOUT : IPedidoQueueOUT
    {
        private readonly IBus _bus;

        public PedidoQueueOUT(RabbitMQConnection connection)
        {
            _bus = connection.bus;
        }

        public Task SendMessageAsync(string queueName, string message)
        {
            return _bus.SendReceive.SendAsync(queueName, message);
        }
    }
}
