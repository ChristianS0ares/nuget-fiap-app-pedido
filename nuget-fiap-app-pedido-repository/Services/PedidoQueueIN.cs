using EasyNetQ;
using nuget_fiap_app_pedido_common.Interfaces.Repository;
using nuget_fiap_app_pedido_common.Interfaces.Services;
using nuget_fiap_app_pedido_repository.Messaging;

namespace nuget_fiap_app_pedido_repository.Services
{
    public class PedidoQueueIN : IPedidoQueueIN, IDisposable
    {
        private readonly IBus _bus;
        private readonly IPedidoService _pedidoService;

        public PedidoQueueIN(RabbitMQConnection connection,
                             IPedidoService pedidoService)
        {
            _bus = connection.bus;
            _pedidoService = pedidoService;
        }
        public void StartListening(string[] queueNames)
        {
            foreach (var queueName in queueNames)
            {
                _bus.SendReceive.Receive<string>(queueName, async message =>
                {
                    await ProcessMessageAsync(queueName, message);
                });
            }
        }
        private async Task ProcessMessageAsync(string queueName, string message)
        {
            switch (queueName)
            { 
                case "pedido-pagamento-recusado":
                    await _pedidoService.AtualizaStatus(message, "Pagamento Recusado");
                    break;
                case "pedido-em-preparacao":
                    await _pedidoService.AtualizaStatus(message, "Em Preparação");
                    break;
                case "pedido-pronto":
                    await _pedidoService.AtualizaStatus(message, "Pronto");
                    break;
                default:
                    Console.WriteLine($"Recebida mensagem da {queueName}: {message}");
                    break;
            }
        }

        public void Dispose()
        {
            _bus.Dispose();
        }
    }
}
