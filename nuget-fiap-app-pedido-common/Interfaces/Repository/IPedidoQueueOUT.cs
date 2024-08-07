using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nuget_fiap_app_pedido_common.Interfaces.Repository
{
    public interface IPedidoQueueOUT
    {
        Task SendMessageAsync(string queueName, string message);
    }
}
