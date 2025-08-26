using Microsoft.AspNetCore.Mvc;
using pandaTeste.api.Application.Interfaces;

namespace pandaTeste.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ViagensController : ControllerBase
    {
        private readonly IViagemService _viagemService;
        public ViagensController(IViagemService viagemService)
        {
            _viagemService = viagemService;
        }

        public ViagensController()
        {

        }

        [HttpGet(Name = "GetViagens")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var viagens = await _viagemService.ObterViagensAgendadasAsync();
                return Ok(viagens);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }

}
