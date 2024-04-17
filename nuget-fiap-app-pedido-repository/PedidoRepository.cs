using MongoDB.Driver;
using nuget_fiap_app_pedido_common.Interfaces.Repository;
using nuget_fiap_app_pedido_common.Models;
using nuget_fiap_app_pedido_repository.DB;

namespace nuget_fiap_app_pedido_repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private RepositoryDB _session;
        private IMongoCollection<Pedido> _pedidos; 
        public PedidoRepository(RepositoryDB session) 
        {
            _session = session;
            _pedidos = _session.Database.GetCollection<Pedido>("pedidos");
        }
        public async Task<string> AddPedido(Pedido pedido)
        {
            if (string.IsNullOrWhiteSpace(pedido.Id))
                pedido.Id = Guid.NewGuid().ToString();

            await _pedidos.InsertOneAsync(pedido);

            return pedido.Id;
        }

        public async Task<bool> DeletePedido(string id)
        {
            var result = await _pedidos.DeleteOneAsync(pedido => pedido.Id == id);

            return result.DeletedCount > 0;
        }

        public async Task<List<Pedido>> GetAllPedidos()
        {
            return await _pedidos.Find(_ => true).ToListAsync();
        }

        public async Task<Pedido> GetPedidoById(string id)
        {
            return await _pedidos.Find(pedido => pedido.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdatePedido(Pedido pedido)
        {
            var filter = Builders<Pedido>.Filter.Eq(p => p.Id, pedido.Id);
            var result = await _pedidos.ReplaceOneAsync(filter, pedido, new ReplaceOptions { IsUpsert = false });

            return result.ModifiedCount > 0;
        }
    }
}
