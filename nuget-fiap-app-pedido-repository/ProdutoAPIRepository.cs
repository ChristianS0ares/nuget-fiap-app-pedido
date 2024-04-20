using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using nuget_fiap_app_pedido_common.Models;
using nuget_fiap_app_pedido_common.Interfaces.Repository;
using nuget_fiap_app_pedido_repository.DTO;

namespace nuget_fiap_app_pedido_repository
{
    public class ProdutoAPIRepository : IProdutoAPIRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly IMemoryCache _cache;
        private readonly string ProdutoCacheKey = "ProdutoCacheKey";

        public ProdutoAPIRepository(HttpClient httpClient, IMemoryCache cache, string baseUrl)
        {
            _httpClient = httpClient;
            //_httpClient.BaseAddress = new Uri(baseUrl);
            _baseUrl = baseUrl;
            _cache = cache;
        }

        public async Task<IEnumerable<Item>> GetAllItens()
        {
            if (_cache.TryGetValue(ProdutoCacheKey, out IEnumerable<Item> produtos))
            {
                return produtos;
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}produto");
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var produtosApi = JsonSerializer.Deserialize<IEnumerable<ProdutoAPI>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (produtosApi == null)
            {
                return new List<Item>();
            }

            produtos = produtosApi.Select(apiProd => new Item
            {
                Id = apiProd.Id,
                Descricao = apiProd.Nome,
                Preco = apiProd.Preco
            }).ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Cache expira após 30 minutos sem acesso

            _cache.Set(ProdutoCacheKey, produtos, cacheEntryOptions);

            return produtos;
        }
    }
}
