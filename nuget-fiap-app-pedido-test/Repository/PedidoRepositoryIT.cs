using nuget_fiap_app_pedido_repository.DB;
using nuget_fiap_app_pedido_repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nuget_fiap_app_pedido_common.Models;
using Xunit;

namespace nuget_fiap_app_pedido_test.Repository
{
    public class PedidoRepositoryIT
    {
        private readonly PedidoRepository _repository;
        private readonly RepositoryDB _repositoryDB;
        public PedidoRepositoryIT()
        {
            _repositoryDB = new RepositoryDB(); 
            _repository = new PedidoRepository(_repositoryDB);
        }

        [Fact]
        public async Task AdicionarPedido_DeveRetornarIdValido()
        {
            var pedido = new Pedido
            {
                Total = 300.00m
            };

            var id = await _repository.AddPedido(pedido);
            Assert.False(string.IsNullOrWhiteSpace(id));

            // Limpa após o teste
            await _repository.DeletePedido(id);
        }

        [Fact]
        public async Task AtualizarPedido_DeveRetornarVerdadeiro()
        {
            var pedido = new Pedido
            {
                Total = 200.00m
            };

            var id = await _repository.AddPedido(pedido);
            pedido.Id = id;
            pedido.Total = 250.00m;

            var updateResult = await _repository.UpdatePedido(pedido);
            Assert.True(updateResult);

            // Limpa após o teste
            await _repository.DeletePedido(id);
        }

        [Fact]
        public async Task DeletarPedido_DeveRetornarVerdadeiro()
        {
            var pedido = new Pedido
            {
                Total = 150.00m
            };

            var id = await _repository.AddPedido(pedido);

            var deleteResult = await _repository.DeletePedido(id);
            Assert.True(deleteResult);
        }

        [Fact]
        public async Task ObterTodosPedidos_DeveRetornarLista()
        {
            var pedido = new Pedido
            {
                Total = 100.00m
            };

            var id = await _repository.AddPedido(pedido);

            var pedidos = await _repository.GetAllPedidos();
            Assert.True(pedidos.Any());

            // Limpa após o teste
            await _repository.DeletePedido(id);
        }

        [Fact]
        public async Task ObterPedidoPorId_DeveRetornarPedido()
        {
            var pedido = new Pedido
            {
                Total = 100.00m
            };

            var id = await _repository.AddPedido(pedido);
            var retrievedPedido = await _repository.GetPedidoById(id);

            Assert.Equal(id, retrievedPedido.Id);
            Assert.Equal(100.00m, retrievedPedido.Total);

            // Limpa após o teste
            await _repository.DeletePedido(id);
        }
    }
}
