using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nuget_fiap_app_pedido_common.Interfaces.Services;
using nuget_fiap_app_pedido_common.Models;
using Swashbuckle.AspNetCore.Annotations;


namespace nuget_fiap_app_pedido.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet(Name = "GetAllPedidos")]
        [SwaggerOperation(Summary = "Listagem de todos os pedidos", Description = "Recupera uma lista de todos os pedidos.")]
        [SwaggerResponse(StatusCodes.Status200OK, "A lista de pedidos foi recuperada com sucesso.", typeof(IEnumerable<Pedido>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nenhum pedido encontrado.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor.")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var pedidos = await _pedidoService.GetAllPedidos();
                if (pedidos == null || pedidos.Count == 0)
                    return NotFound("Nenhum pedido encontrado.");

                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}", Name = "GetPedidoById")]
        [SwaggerOperation(Summary = "Obtenção de pedido por ID", Description = "Obtém um pedido com base no ID especificado.")]
        [SwaggerResponse(StatusCodes.Status200OK, "O pedido foi recuperado com sucesso.", typeof(Pedido))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Pedido não encontrado.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor.")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var pedido = await _pedidoService.GetPedidoById(id);
                if (pedido == null)
                    return NotFound($"Pedido com ID {id} não encontrado.");

                return Ok(pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost(Name = "CreatePedido")]
        [SwaggerOperation(Summary = "Criação de um novo pedido", Description = "Cria um novo pedido com base nos dados fornecidos.")]
        [SwaggerResponse(StatusCodes.Status201Created, "O pedido foi criado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor.")]
        public async Task<IActionResult> Post([FromBody] Pedido pedido)
        {
            try
            {
                var pedidoId = await _pedidoService.AddPedido(pedido);
                return CreatedAtRoute("GetPedidoById", new { id = pedidoId }, pedido);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}", Name = "UpdatePedido")]
        [SwaggerOperation(Summary = "Atualização de um pedido por ID", Description = "Atualiza um pedido com base no ID especificado.")]
        [SwaggerResponse(StatusCodes.Status200OK, "O pedido foi atualizado com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Pedido não encontrado.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor.")]
        public async Task<IActionResult> Put(string id, [FromBody] Pedido pedido)
        {
            try
            {
                bool updated = await _pedidoService.UpdatePedido(pedido, id);
                if (!updated)
                    return NotFound($"Pedido com ID {id} não encontrado.");

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}", Name = "DeletePedido")]
        [SwaggerOperation(Summary = "Exclusão de pedido por ID", Description = "Exclui um pedido com base no ID especificado.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "O pedido foi excluído com sucesso.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Pedido não encontrado.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor.")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                bool deleted = await _pedidoService.DeletePedido(id);
                if (!deleted)
                    return NotFound($"Pedido com ID {id} não encontrado.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
