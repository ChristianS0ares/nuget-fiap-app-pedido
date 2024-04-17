using nuget_fiap_app_pedido_common.Models;

namespace nuget_fiap_app_pedido_common.Interfaces.Repository
{
    public interface IPedidoRepository
    {
        Task<List<Pedido>> GetAllPedidos();
        Task<Pedido> GetPedidoById(string id);
        Task<string> AddPedido(Pedido pedido);
        Task<bool> UpdatePedido(Pedido pedido);
        Task<bool> DeletePedido(string id);
    }
}
