using nuget_fiap_app_pedido_common.Interfaces.Repository;
using nuget_fiap_app_pedido_common.Interfaces.Services;
using nuget_fiap_app_pedido_common.Models;
using System.Transactions;

namespace nuget_fiap_app_pedido.Service
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProdutoAPIRepository _produtoRepository;
        private readonly IPedidoQueueOUT _pedidoQueueOUT;
        

        public PedidoService(IPedidoRepository pedidoRepository, IProdutoAPIRepository produtoRepository, IPedidoQueueOUT pedidoQueueOUT)
        {
            _pedidoRepository = pedidoRepository;
            _produtoRepository = produtoRepository;
            _pedidoQueueOUT = pedidoQueueOUT;
        }

        private async Task<List<Item>> ValidateAndEnrichPedidoItems(IEnumerable<Item> items)
        {
            var produtos = await _produtoRepository.GetAllItens();
            var produtoDict = produtos.ToDictionary(p => p.Id);

            var enrichedItems = new List<Item>();
            foreach (var item in items)
            {
                if (!produtoDict.ContainsKey(item.Id))
                    throw new InvalidOperationException($"Produto com ID {item.Id} não encontrado no catálogo.");

                var produto = produtoDict[item.Id];
                enrichedItems.Add(new Item
                {
                    Id = item.Id,
                    Descricao = produto.Descricao, // Atualiza a descrição com a do catálogo
                    Quantidade = item.Quantidade,
                    Preco = produto.Preco // Atualiza o preço com a do catálogo
                });
            }
            return enrichedItems;
        }

        public async Task<string> AddPedido(Pedido pedido)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                pedido.Data = DateTime.Now;
                pedido.Status = "Recebido";
                pedido.Itens = await ValidateAndEnrichPedidoItems(pedido.Itens);
                string pedidoId = await _pedidoRepository.AddPedido(pedido);
                await _pedidoQueueOUT.SendMessageAsync("pedido-recebido", pedidoId);

                scope.Complete();

                return pedidoId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AtualizaStatus(string id, string status)
        {
            var pedido = await GetPedidoById(id);
            pedido.Status = status;
            return await _pedidoRepository.UpdatePedido(pedido);
        }

        public async Task<bool> UpdatePedido(Pedido pedido, string id)
        {
            var existingPedido = await _pedidoRepository.GetPedidoById(id);
            if (existingPedido == null)
            {
                return false;
            }

            var enrichedItems = await ValidateAndEnrichPedidoItems(pedido.Itens);
            existingPedido.Data = pedido.Data;
            existingPedido.Itens = enrichedItems;

            return await _pedidoRepository.UpdatePedido(existingPedido);
        }

        public async Task<bool> DeletePedido(string id)
        {
            return await _pedidoRepository.DeletePedido(id);
        }

        public async Task<List<Pedido>> GetAllPedidos()
        {
            return await _pedidoRepository.GetAllPedidos();
        }

        public async Task<Pedido> GetPedidoById(string id)
        {
            return await _pedidoRepository.GetPedidoById(id);
        }
    }
}
