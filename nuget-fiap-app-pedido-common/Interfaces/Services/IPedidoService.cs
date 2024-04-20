using nuget_fiap_app_pedido_common.Models;

namespace nuget_fiap_app_pedido_common.Interfaces.Services
{
    public interface IPedidoService
    {
        Task<List<Pedido>> GetAllPedidos();
        Task<Pedido> GetPedidoById(string id);
        Task<string> AddPedido(Pedido pedido);
        Task<bool> UpdatePedido(Pedido pedido, string id);
        Task<bool> DeletePedido(string id);
    }
}
