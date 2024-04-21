using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using nuget_fiap_app_pedido_common.Models;
using System.Net;
using System.Text;
using Xunit;

namespace nuget_fiap_app_pedido_test.Controller
{
    public class PedidoControllerIT : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PedidoControllerIT(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> CreatePedidoAsync()
        {
            var newPedido = new Pedido
            {
                Cliente = new Cliente { Nome = "John Doe", CPF = "12345678901", Email = "john@example.com" },
                Itens = new List<Item> { new Item { Id = 1, Descricao = "Hambúrguer", Quantidade = 10 } }
            };
            var content = new StringContent(JsonConvert.SerializeObject(newPedido), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/Pedido", content);
            response.EnsureSuccessStatusCode();

            var location = response.Headers.Location.ToString();
            var id = location.Substring(location.LastIndexOf('/') + 1);
            return id;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetAllPedidos_ShouldReturn200OK_WhenPedidosExist()
        {
            var id = await CreatePedidoAsync();
            var response = await _client.GetAsync("/Pedido");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetPedidoById_ShouldReturn200OK_WhenPedidoExists()
        {
            var id = await CreatePedidoAsync();
            var response = await _client.GetAsync($"/Pedido/{id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UpdatePedido_ShouldReturn200OK_WhenPedidoIsUpdated()
        {
            var id = await CreatePedidoAsync();
            var updatedPedido = new Pedido
            {
                Cliente = new Cliente { Nome = "Jane Doe", CPF = "12345678901", Email = "jane@example.com" },
                Itens = new List<Item> { new Item { Id = 1, Descricao = "Hambúrguer", Quantidade = 5 } }
            };
            var content = new StringContent(JsonConvert.SerializeObject(updatedPedido), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/Pedido/{id}", content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task DeletePedido_ShouldReturn204NoContent_WhenPedidoIsDeleted()
        {
            var id = await CreatePedidoAsync();
            var response = await _client.DeleteAsync($"/Pedido/{id}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task DeletePedido_ShouldReturn404NotFound_WhenPedidoDoesNotExist()
        {
            var response = await _client.DeleteAsync("/Pedido/999999"); // Assumindo que este ID não existe
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
