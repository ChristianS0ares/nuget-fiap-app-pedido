using nuget_fiap_app_pedido_common.Models;

namespace nuget_fiap_app_pedido_common.Interfaces.Repository
{
    public interface IProdutoAPIRepository
    {
        public Task<IEnumerable<Item>> GetAllItens();
    }
}
