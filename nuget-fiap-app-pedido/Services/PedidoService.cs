using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using nuget_fiap_app_pedido_common.Interfaces.Repository;
using nuget_fiap_app_pedido_common.Interfaces.Services;
using nuget_fiap_app_pedido_common.Models;

namespace nuget_fiap_app_pedido.Service
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProdutoAPIRepository _produtoRepository;

        public PedidoService(IPedidoRepository pedidoRepository, IProdutoAPIRepository produtoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _produtoRepository = produtoRepository;
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
            pedido.Data = DateTime.Now;
            pedido.Itens = await ValidateAndEnrichPedidoItems(pedido.Itens);
            string pedidoId = await _pedidoRepository.AddPedido(pedido);
            return pedidoId;
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
