using FluentAssertions;
using nuget_fiap_app_pedido_common.Interfaces.Repository;
using nuget_fiap_app_pedido_common.Models;
using nuget_fiap_app_pedido_repository.DB;
using nuget_fiap_app_pedido_repository;
using Xunit;
using nuget_fiap_app_pedido.Service;
using nuget_fiap_app_pedido_repository.Interface;
using Microsoft.Extensions.Caching.Memory;
using nuget_fiap_app_pedido_repository.Services;
using nuget_fiap_app_pedido_repository.Messaging;

namespace nuget_fiap_app_pedido_test.Service
{
    public class PedidoServiceIT
    {
        private readonly PedidoService _pedidoService;
        private readonly RepositoryDB _repositoryDB;
        private readonly RabbitMQConnection _rabbitmqConnection;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProdutoAPIRepository _produtoRepository;
        private readonly IPedidoQueueOUT _pedidoQueueOUT;

        public PedidoServiceIT()
        {
            var httpClient = new HttpClient();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var baseUrl = "http://localhost:8080/";//Usa o serviço local para teste

            _repositoryDB = new RepositoryDB(); 
            _pedidoRepository = new PedidoRepository(_repositoryDB);
            _produtoRepository = new ProdutoAPIRepository(httpClient, memoryCache, baseUrl);
            _rabbitmqConnection = new RabbitMQConnection();
            _pedidoQueueOUT = new PedidoQueueOUT(_rabbitmqConnection);
            _pedidoService = new PedidoService(_pedidoRepository, _produtoRepository, _pedidoQueueOUT);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task DevePermitirCriarPedido()
        {
            var novoPedido = new Pedido
            {
                Itens = new List<Item>
                {
                    new Item { Id = 1, Quantidade = 10, Descricao = "Hambúrguer", Preco = 5.50m }
                }
            };

            var pedidoId = await _pedidoService.AddPedido(novoPedido);

            pedidoId.Should().NotBeNullOrEmpty();

            var pedidoCriado = await _pedidoService.GetPedidoById(pedidoId);

            pedidoCriado.Should().NotBeNull();
            pedidoCriado.Itens.Should().HaveCount(1);
            pedidoCriado.Itens.First().Descricao.Should().Be("Hambúrguer");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task DevePermitirAtualizarPedido()
        {
            var pedido = new Pedido
            {
                Itens = new List<Item>
                {
                    new Item { Id = 1, Quantidade = 5, Descricao = "Hambúrguer", Preco = 3.75m }
                }
            };

            var pedidoId = await _pedidoService.AddPedido(pedido);
            pedido.Id = pedidoId; // Atualizar o ID para garantir consistência
            pedido.Itens[0].Quantidade = 2;

            var resultado = await _pedidoService.UpdatePedido(pedido, pedidoId);

            resultado.Should().BeTrue();

            var pedidoAtualizado = await _pedidoService.GetPedidoById(pedidoId);

            pedidoAtualizado.Should().NotBeNull();
            pedidoAtualizado.Itens.First().Quantidade.Should().Be(2);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task DevePermitirExcluirPedido()
        {
            var pedido = new Pedido
            {
                Itens = new List<Item>
                {
                    new Item { Id = 1, Quantidade = 10, Descricao = "Item para exclusão", Preco = 5.50m }
                }
            };

            var pedidoId = await _pedidoService.AddPedido(pedido);

            var resultadoExclusao = await _pedidoService.DeletePedido(pedidoId);

            resultadoExclusao.Should().BeTrue();

            var pedidoExcluido = await _pedidoService.GetPedidoById(pedidoId);

            pedidoExcluido.Should().BeNull();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task DeveRecuperarTodosPedidos()
        {
            await _pedidoService.AddPedido(new Pedido
            {
                Itens = new List<Item>
                {
                    new Item { Id = 1, Quantidade = 10, Descricao = "Hambúrguer", Preco = 5.50m }
                }
            });

            await _pedidoService.AddPedido(new Pedido
            {
                Itens = new List<Item>
                {
                    new Item { Id = 2, Quantidade = 20, Descricao = "Batata Frita", Preco = 15.00m }
                }
            });

            var pedidos = await _pedidoService.GetAllPedidos();

            pedidos.Should().NotBeNull();
            pedidos.Should().HaveCountGreaterOrEqualTo(2);
            pedidos.SelectMany(p => p.Itens).Select(i => i.Descricao).Should().Contain(new[] { "Hambúrguer", "Batata Frita" });
        }
    }
}
