using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using nuget_fiap_app_pedido.Service;
using nuget_fiap_app_pedido_common.Interfaces.Repository;
using nuget_fiap_app_pedido_common.Interfaces.Services;
using nuget_fiap_app_pedido_repository;
using nuget_fiap_app_pedido_repository.DB;
using Microsoft.Extensions.Caching.Memory;
using nuget_fiap_app_pedido_repository.Interface;
using nuget_fiap_app_pedido_repository.Services;
using nuget_fiap_app_pedido_repository.Messaging;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Adicione configuração do MemoryCache
        builder.Services.AddMemoryCache();
        var configuration = builder.Configuration;

        // Configurando ProdutoAPIRepository com HttpClient e parâmetros necessários
        builder.Services.AddHttpClient("ProdutoAPI", client =>
        {
            client.BaseAddress = new Uri(configuration["ProdutoApi:BaseUrl"]);
        });

        // Registra o ProdutoAPIRepository com os parâmetros requeridos incluindo IMemoryCache e baseUrl
        builder.Services.AddScoped<IProdutoAPIRepository, ProdutoAPIRepository>(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("ProdutoAPI");
            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            var baseUrl = configuration["ProdutoApi:BaseUrl"];
            return new ProdutoAPIRepository(httpClient, memoryCache, baseUrl);
        });

        // Registro de outros serviços e repositórios
        builder.Services.AddScoped<RepositoryDB>();
        builder.Services.AddScoped<RabbitMQConnection>();
        builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
        builder.Services.AddScoped<IPedidoService, PedidoService>();
        builder.Services.AddScoped<IPedidoQueueOUT, PedidoQueueOUT>();
        builder.Services.AddScoped<IPedidoQueueIN, PedidoQueueIN>();
        builder.Services.AddMemoryCache();

        // Configuração do HealthCheck e Swagger
        builder.Services.AddHealthChecks();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "NuGET Burger",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Miro",
                    Url = new Uri("https://miro.com/app/board/uXjVMqYSzbg=/?share_link_id=124875092732")
                }
            });
        });

        var app = builder.Build();

        // Configuração do pipeline de requisições HTTP
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "NuGET Burger API V1");
        });
        app.UseReDoc(c =>
        {
            c.DocumentTitle = "REDOC API Documentation";
            c.SpecUrl("/swagger/v1/swagger.json");
        });

        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health", new HealthCheckOptions()
        {
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
            },
        });

        // Criar um escopo para resolver IPedidoQueueIN
        using (var scope = app.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var pedidoQueueIN = scopedServices.GetRequiredService<IPedidoQueueIN>();
            pedidoQueueIN.StartListening(new string[] { "pedido-pagamento-recusado", "pedido-em-preparacao", "pedido-pronto" });
        }
        app.Run();
    }
}
