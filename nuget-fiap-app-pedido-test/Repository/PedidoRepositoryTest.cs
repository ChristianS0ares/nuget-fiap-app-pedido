using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using nuget_fiap_app_pedido_common.Models;
using nuget_fiap_app_pedido_common.Interfaces.Repository;

namespace nuget_fiap_app_pedido_test.Repository
{
    public class PedidoRepositoryTest
    {
        private readonly Mock<IPedidoRepository> _mockPedidoRepository;

        public PedidoRepositoryTest()
        {
            _mockPedidoRepository = new Mock<IPedidoRepository>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task AdicionarPedido_DeveRetornarId()
        {
            var novoPedido = new Pedido
            {
                Itens = new List<Item>
                {
                    new Item { Id = 1, Descricao = "Item 1", Preco = 100.00m, Quantidade = 2 }
                }
            };

            _mockPedidoRepository.Setup(repo => repo.AddPedido(It.IsAny<Pedido>()))
                                 .ReturnsAsync(Guid.NewGuid().ToString());

            var idResultado = await _mockPedidoRepository.Object.AddPedido(novoPedido);

            Assert.NotNull(idResultado);
            Assert.NotEmpty(idResultado);
            _mockPedidoRepository.Verify(repo => repo.AddPedido(It.IsAny<Pedido>()), Times.Once);

            // Verifica se o total calculado é correto
            Assert.Equal(200m, novoPedido.Total);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeletarPedido_DeveRetornarVerdadeiro()
        {
            string idPedido = Guid.NewGuid().ToString();
            _mockPedidoRepository.Setup(repo => repo.DeletePedido(It.IsAny<string>()))
                                 .ReturnsAsync(true);

            var resultado = await _mockPedidoRepository.Object.DeletePedido(idPedido);

            Assert.True(resultado);
            _mockPedidoRepository.Verify(repo => repo.DeletePedido(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ObterTodosPedidos_DeveRetornarListaDePedidos()
        {
            var listaPedidos = new List<Pedido>
            {
                new Pedido { Itens = new List<Item> { new Item { Id = 1, Descricao = "Item 1", Preco = 50.00m, Quantidade = 2 } } },
                new Pedido { Itens = new List<Item> { new Item { Id = 2, Descricao = "Item 2", Preco = 75.00m, Quantidade = 2 } } }
            };
            _mockPedidoRepository.Setup(repo => repo.GetAllPedidos())
                                 .ReturnsAsync(listaPedidos);

            var resultado = await _mockPedidoRepository.Object.GetAllPedidos();

            Assert.Equal(2, resultado.Count);
            _mockPedidoRepository.Verify(repo => repo.GetAllPedidos(), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ObterPedidoPorId_DeveRetornarPedido()
        {
            var pedido = new Pedido
            {
                Id = Guid.NewGuid().ToString(),
                Itens = new List<Item> { new Item { Id = 1, Descricao = "Item Test", Preco = 100.00m, Quantidade = 1 } }
            };
            _mockPedidoRepository.Setup(repo => repo.GetPedidoById(It.IsAny<string>()))
                                 .ReturnsAsync(pedido);

            var resultado = await _mockPedidoRepository.Object.GetPedidoById(pedido.Id);

            Assert.Equal(pedido.Id, resultado.Id);
            _mockPedidoRepository.Verify(repo => repo.GetPedidoById(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task AtualizarPedido_DeveRetornarVerdadeiro()
        {
            var pedido = new Pedido
            {
                Id = Guid.NewGuid().ToString(),
                Itens = new List<Item> { new Item { Id = 1, Descricao = "Item Atualizado", Preco = 300.00m, Quantidade = 1 } }
            };
            _mockPedidoRepository.Setup(repo => repo.UpdatePedido(It.IsAny<Pedido>()))
                                 .ReturnsAsync(true);

            var resultado = await _mockPedidoRepository.Object.UpdatePedido(pedido);

            Assert.True(resultado);
            _mockPedidoRepository.Verify(repo => repo.UpdatePedido(It.IsAny<Pedido>()), Times.Once);
        }
    }
}
