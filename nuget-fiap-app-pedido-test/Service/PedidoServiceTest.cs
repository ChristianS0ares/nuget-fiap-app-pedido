using Moq;
using Xunit;
using nuget_fiap_app_pedido_common.Interfaces.Repository;
using nuget_fiap_app_pedido_common.Models;
using nuget_fiap_app_pedido.Service;


namespace nuget_fiap_app_pedido_test.Service
{
    public class PedidoServiceTests
    {
        private readonly Mock<IPedidoRepository> _mockPedidoRepository = new Mock<IPedidoRepository>();
        private readonly Mock<IProdutoAPIRepository> _mockProdutoRepository = new Mock<IProdutoAPIRepository>();
        private readonly PedidoService _pedidoService;

        public PedidoServiceTests()
        {
            _pedidoService = new PedidoService(_mockPedidoRepository.Object, _mockProdutoRepository.Object);
        }

        [Fact]
        public async Task AddPedido_ReturnsPedidoId_WhenValidItems()
        {
            // Arrange
            var pedido = new Pedido { Itens = new List<Item> { new Item { Id = 1, Quantidade = 2 } } };
            var produtos = new List<Item> { new Item { Id = 1, Descricao = "Produto 1", Preco = 10.0m } };

            _mockProdutoRepository.Setup(repo => repo.GetAllItens()).ReturnsAsync(produtos);
            _mockPedidoRepository.Setup(repo => repo.AddPedido(It.IsAny<Pedido>())).ReturnsAsync("123");

            // Act
            var result = await _pedidoService.AddPedido(pedido);

            // Assert
            Assert.Equal("123", result);
        }

        [Fact]
        public async Task UpdatePedido_ReturnsFalse_WhenPedidoDoesNotExist()
        {
            // Arrange
            var pedido = new Pedido { Itens = new List<Item> { new Item { Id = 1, Quantidade = 2 } } };
            var id = "non-existing-id";

            _mockPedidoRepository.Setup(repo => repo.GetPedidoById(id)).ReturnsAsync((Pedido)null);

            // Act
            var result = await _pedidoService.UpdatePedido(pedido, id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdatePedido_ReturnsTrue_WhenPedidoExists()
        {
            // Arrange
            var pedido = new Pedido { Itens = new List<Item> { new Item { Id = 1, Quantidade = 2 } } };
            var existingPedido = new Pedido { Id = "123", Itens = new List<Item> { new Item { Id = 1, Quantidade = 1 } } };
            var produtos = new List<Item> { new Item { Id = 1, Descricao = "Produto 1", Preco = 10.0m } };

            _mockPedidoRepository.Setup(repo => repo.GetPedidoById("123")).ReturnsAsync(existingPedido);
            _mockProdutoRepository.Setup(repo => repo.GetAllItens()).ReturnsAsync(produtos);
            _mockPedidoRepository.Setup(repo => repo.UpdatePedido(It.IsAny<Pedido>())).ReturnsAsync(true);

            // Act
            var result = await _pedidoService.UpdatePedido(pedido, "123");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeletePedido_ReturnsTrue_WhenPedidoDeleted()
        {
            // Arrange
            _mockPedidoRepository.Setup(repo => repo.DeletePedido("123")).ReturnsAsync(true);

            // Act
            var result = await _pedidoService.DeletePedido("123");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetAllPedidos_ReturnsPedidos()
        {
            // Arrange
            var pedidos = new List<Pedido> { new Pedido { Id = "123" } };
            _mockPedidoRepository.Setup(repo => repo.GetAllPedidos()).ReturnsAsync(pedidos);

            // Act
            var result = await _pedidoService.GetAllPedidos();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetPedidoById_ReturnsPedido()
        {
            // Arrange
            var pedido = new Pedido { Id = "123" };
            _mockPedidoRepository.Setup(repo => repo.GetPedidoById("123")).ReturnsAsync(pedido);

            // Act
            var result = await _pedidoService.GetPedidoById("123");

            // Assert
            Assert.Equal("123", result.Id);
        }
    }
}
