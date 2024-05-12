using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using nuget_fiap_app_pedido_common.Interfaces.Services;
using nuget_fiap_app_pedido_common.Models;
using nuget_fiap_app_pedido.Controllers;

namespace nuget_fiap_app_pedido_test.Controller
{
    public class PedidoControllerTest
    {
        private readonly Mock<IPedidoService> _pedidoServiceMock;
        private readonly PedidoController _controller;

        public PedidoControllerTest()
        {
            _pedidoServiceMock = new Mock<IPedidoService>();
            _controller = new PedidoController(_pedidoServiceMock.Object);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAll_ShouldReturn200OK_WhenPedidosExist()
        {
            var pedidos = new List<Pedido>
            {
                new Pedido { Id = "1", Itens = new List<Item> { new Item { Id = 1, Preco = 100, Quantidade = 1 } } },
                new Pedido { Id = "2", Itens = new List<Item> { new Item { Id = 2, Preco = 50, Quantidade = 2 } } }
            };
            _pedidoServiceMock.Setup(x => x.GetAllPedidos()).ReturnsAsync(pedidos);

            var result = await _controller.GetAll();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(pedidos);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAll_ShouldReturn404NotFound_WhenNoPedidosExist()
        {
            _pedidoServiceMock.Setup(x => x.GetAllPedidos()).ReturnsAsync(new List<Pedido>());

            var result = await _controller.GetAll();

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetById_ShouldReturn200OK_WhenPedidoExists()
        {
            var pedido = new Pedido { Id = "1" };
            _pedidoServiceMock.Setup(x => x.GetPedidoById("1")).ReturnsAsync(pedido);

            var result = await _controller.GetById("1");

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(pedido);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetById_ShouldReturn404NotFound_WhenPedidoDoesNotExist()
        {
            _pedidoServiceMock.Setup(x => x.GetPedidoById("1")).ReturnsAsync((Pedido)null);

            var result = await _controller.GetById("1");

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Post_ShouldReturn201Created_WhenPedidoIsCreated()
        {
            var pedido = new Pedido();
            _pedidoServiceMock.Setup(x => x.AddPedido(It.IsAny<Pedido>())).ReturnsAsync("1");

            var result = await _controller.Post(pedido);

            result.Should().BeOfType<CreatedAtRouteResult>();
            var createdResult = result as CreatedAtRouteResult;
            createdResult.RouteName.Should().Be("GetPedidoById");
            createdResult.RouteValues["id"].Should().Be("1");
            createdResult.Value.Should().BeEquivalentTo(pedido);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Put_ShouldReturn200OK_WhenPedidoIsUpdated()
        {
            _pedidoServiceMock.Setup(x => x.UpdatePedido(It.IsAny<Pedido>(), "1")).ReturnsAsync(true);

            var result = await _controller.Put("1", new Pedido());

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Put_ShouldReturn404NotFound_WhenPedidoDoesNotExist()
        {
            _pedidoServiceMock.Setup(x => x.UpdatePedido(It.IsAny<Pedido>(), "1")).ReturnsAsync(false);

            var result = await _controller.Put("1", new Pedido());

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Delete_ShouldReturn204NoContent_WhenPedidoIsDeleted()
        {
            _pedidoServiceMock.Setup(x => x.DeletePedido("1")).ReturnsAsync(true);

            var result = await _controller.Delete("1");

            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Delete_ShouldReturn404NotFound_WhenPedidoDoesNotExist()
        {
            _pedidoServiceMock.Setup(x => x.DeletePedido("1")).ReturnsAsync(false);

            var result = await _controller.Delete("1");

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetAll_ShouldReturn500InternalServerError_WhenExceptionIsThrown()
        {
            _pedidoServiceMock.Setup(x => x.GetAllPedidos()).ThrowsAsync(new Exception("Internal Server Error"));

            var result = await _controller.GetAll();

            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(500);
        }
    }
}
