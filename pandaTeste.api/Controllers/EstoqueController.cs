using Microsoft.AspNetCore.Mvc;
using pandaTeste.api.Application.Interfaces;

namespace pandaTeste.api.Controllers
{
    [ApiController]
    [Route("api/estoque")]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueService _service;

        public EstoqueController(IEstoqueService service)
        {
            _service = service;
        }

        [HttpPost("adicionar")]
        public IActionResult Adicionar([FromQuery] int itemId, [FromQuery] string nomeItem, [FromQuery] int quantidade)
        {
            _service.AdicionarAoEstoque(itemId, nomeItem, quantidade);
            return Ok();
        }

        [HttpPost("entregar")]
        public IActionResult Entregar([FromQuery] int itemId, [FromQuery] int quantidade)
        {
            var sucesso = _service.EntregarDoEstoque(itemId, quantidade);
            return sucesso ? Ok() : BadRequest("Quantidade insuficiente ou item não encontrado");
        }
    }
}
