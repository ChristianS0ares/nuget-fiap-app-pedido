using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using nuget_fiap_app_pedido_common.Interfaces.Services;
using nuget_fiap_app_pedido_common.Models;
using nuget_fiap_app_pedido.Controllers;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

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
        public async Task GetAll_ShouldReturn200OK_WhenPedidosExist()
        {
            // Arrange
            var pedidos = new List<Pedido>
            {
                new Pedido { Id = "1" },
                new Pedido { Id = "2" }
            };
            _pedidoServiceMock.Setup(service => service.GetAllPedidos()).ReturnsAsync(pedidos);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedPedidos = okResult.Value.Should().BeAssignableTo<IEnumerable<Pedido>>().Subject;
            returnedPedidos.Should().HaveCount(pedidos.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturn200OK_WhenPedidoExists()
        {
            // Arrange
            var pedido = new Pedido { Id = "1" };
            _pedidoServiceMock.Setup(service => service.GetPedidoById("1")).ReturnsAsync(pedido);

            // Act
            var result = await _controller.GetById("1");

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(pedido);
        }

        [Fact]
        public async Task Post_ShouldReturn201Created_WhenPedidoIsCreated()
        {
            // Arrange
            var newPedido = new Pedido { Total = 200 };
            _pedidoServiceMock.Setup(service => service.AddPedido(newPedido)).ReturnsAsync("3");

            // Act
            var result = await _controller.Post(newPedido);

            // Assert
            var createdAtRouteResult = result.Should().BeOfType<CreatedAtRouteResult>().Subject;
            createdAtRouteResult.RouteName.Should().Be("GetPedidoById");
            createdAtRouteResult.RouteValues["id"].Should().Be("3");
            createdAtRouteResult.Value.Should().BeEquivalentTo(newPedido);
        }

        [Fact]
        public async Task Put_ShouldReturn200OK_WhenPedidoIsUpdated()
        {
            // Arrange
            var existingPedido = new Pedido { Id = "1", Total = 250 };
            _pedidoServiceMock.Setup(service => service.UpdatePedido(existingPedido, "1")).ReturnsAsync(true);

            // Act
            var result = await _controller.Put("1", existingPedido);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturn204NoContent_WhenPedidoIsDeleted()
        {
            // Arrange
            _pedidoServiceMock.Setup(service => service.DeletePedido("1")).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete("1");

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
