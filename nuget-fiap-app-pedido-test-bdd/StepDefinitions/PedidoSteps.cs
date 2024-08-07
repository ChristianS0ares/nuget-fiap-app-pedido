using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using nuget_fiap_app_pedido_common.Models;
using System.Net.Http.Json;
using TechTalk.SpecFlow;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nuget_fiap_app_pedido_test.BDD
{
    [Binding]
    public class PedidoSteps
    {
        private readonly HttpClient _client;
        private HttpResponseMessage _response;
        private Pedido _pedidoCriado;
        private readonly string _baseUrl = "/pedido";

        public PedidoSteps(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Given(@"que eu criei um pedido com o cliente ""(.*)""")]
        public async Task GivenQueEuCrieiUmPedidoComOCliente(string nomeCliente)
        {
            var novoPedido = new Pedido
            {
                Cliente = new Cliente { Nome = nomeCliente },
                Itens = new List<Item> { new Item { Id = 1, Descricao = "Hambúrguer", Quantidade = 1 } },
                Data = System.DateTime.Now
            };
            _response = await _client.PostAsJsonAsync(_baseUrl, novoPedido);
            _response.EnsureSuccessStatusCode();

            var locationHeader = _response.Headers.Location.ToString();
            var pedidoId = locationHeader.Split('/').Last();

            _response = await _client.GetAsync($"{_baseUrl}/{pedidoId}");
            _response.EnsureSuccessStatusCode();

            _pedidoCriado = await _response.Content.ReadFromJsonAsync<Pedido>();
            _pedidoCriado.Should().NotBeNull();
            _pedidoCriado.Id.Should().Be(pedidoId);
        }

        [Given(@"que eu tenho itens disponíveis para pedido")]
        public void GivenQueEuTenhoItensDisponiveisParaPedido()
        {
            // This step assumes that items are available for creating orders.
        }

        [When(@"eu adiciono um pedido com os itens ""(.*)"" e ""(.*)""")]
        public async Task WhenEuAdicionoUmPedidoComOsItens(int item1, int item2)
        {
            var novoPedido = new Pedido
            {
                Cliente = new Cliente { Nome = "Cliente Novo" },
                Itens = new List<Item>
                {
                    new Item { Id = item1, Descricao = "Item " + item1, Quantidade = 1, Preco = 5.00m },
                    new Item { Id = item2, Descricao = "Item " + item2, Quantidade = 1, Preco = 15.00m }
                },
                Data = System.DateTime.Now
            };
            _response = await _client.PostAsJsonAsync(_baseUrl, novoPedido);
            _response.EnsureSuccessStatusCode();

            var locationHeader = _response.Headers.Location.ToString();
            var pedidoId = locationHeader.Split('/').Last();
            _response = await _client.GetAsync($"{_baseUrl}/{pedidoId}");
            _response.EnsureSuccessStatusCode();

            _pedidoCriado = await _response.Content.ReadFromJsonAsync<Pedido>();
            _pedidoCriado.Should().NotBeNull();
            _pedidoCriado.Id.Should().Be(pedidoId);
        }

        [When(@"eu solicito o pedido pelo seu ID")]
        public async Task WhenEuSolicitoOPedidoPeloSeuID()
        {
            _response = await _client.GetAsync($"{_baseUrl}/{_pedidoCriado.Id}");
        }

        [When(@"eu atualizo o pedido para incluir o item ""(.*)"" e excluir o item ""(.*)""")]
        public async Task WhenEuAtualizoOPedidoParaIncluirOItemEExcluirOItem(int incluir, int excluir)
        {
            var pedidoAtualizado = new Pedido
            {
                Cliente = _pedidoCriado.Cliente,
                Itens = new List<Item> { new Item { Id = incluir, Descricao = "Item " + incluir, Quantidade = 2 } }
            };
            _response = await _client.PutAsJsonAsync($"{_baseUrl}/{_pedidoCriado.Id}", pedidoAtualizado);
        }

        [When(@"eu excluo o pedido do cliente ""(.*)""")]
        public async Task WhenEuExcluoOPedidoDoCliente(string nomeCliente)
        {
            _response = await _client.DeleteAsync($"{_baseUrl}/{_pedidoCriado.Id}");
        }

        [When(@"eu tento atualizar um pedido com o ID inexistente ""(.*)""")]
        public async Task WhenEuTentoOperarComUmIDInexistente(string pedidoId)
        {
            var pedidoAtualizado = new Pedido
            {
                Cliente = new Cliente { Nome = "Cliente Inexistente" },
                Itens = new List<Item> { new Item { Id = 999, Descricao = "Item Inexistente", Quantidade = 1, Preco = 10.00m } },
                Data = System.DateTime.Now
            };
            _response = await _client.PutAsJsonAsync($"{_baseUrl}/{pedidoId}", pedidoAtualizado);
        }

        [When(@"eu tento excluir um pedido com o ID inexistente ""(.*)""")]
        public async Task WhenEuTentoExcluirComUmIDInexistente(string pedidoId)
        {
            _response = await _client.DeleteAsync($"{_baseUrl}/{pedidoId}");
        }

        [When(@"eu solicito a lista de pedidos")]
        public async Task WhenEuSolicitoAListagemDePedidos()
        {
            _response = await _client.GetAsync($"{_baseUrl}");
        }

        [Then(@"eu devo receber uma lista contendo o pedido do cliente ""(.*)""")]
        public async Task ThenEuDevoReceberUmaListaContendoOPedidoDoCliente(string nomeCliente)
        {
            _response.EnsureSuccessStatusCode();
            var pedidos = await _response.Content.ReadFromJsonAsync<List<Pedido>>();
            pedidos.Should().Contain(pedido => pedido.Cliente != null && pedido.Cliente.Nome == nomeCliente);

        }

        [Then(@"o pedido deve ser adicionado com sucesso e contendo os itens ""(.*)"" e ""(.*)""")]
        public async void ThenOPedidoDeveSerAdicionadoComSucessoEContendoOsItens(int item1, int item2)
        {
            _pedidoCriado.Should().NotBeNull();
            _pedidoCriado.Itens[0].Id.Should().Be(item1);
            _pedidoCriado.Itens[1].Id.Should().Be(item2);
        }

        [Then(@"eu devo receber o pedido do cliente ""(.*)""")]
        public async Task ThenEuDevoReceberOPedidoDoCliente(string nomeCliente)
        {
            _response.EnsureSuccessStatusCode();
            var pedido = await _response.Content.ReadFromJsonAsync<Pedido>();
            pedido.Cliente.Nome.Should().Be(nomeCliente);
        }

        [Then(@"eu devo receber o pedido atualizado com o item ""(.*)""")]
        public async Task ThenEuDevoReceberOPedidoAtualizadoComOItem(int item)
        {
            _response.EnsureSuccessStatusCode();
            var pedido = await _response.Content.ReadFromJsonAsync<Pedido>();
            pedido.Itens.Should().Contain(i => i.Id == item);
        }

        [Then(@"o pedido do cliente ""(.*)"" não deve mais existir")]
        public void ThenOPedidoDoClienteNaoDeveMaisExistir(string nomeCliente)
        {
            _response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Then(@"eu devo receber uma mensagem de erro informando que o pedido não existe")]
        public void ThenEuDevoReceberUmaMensagemDeErroInformandoQueOPedidoNaoExiste()
        {
            _response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
