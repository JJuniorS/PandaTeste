using Microsoft.AspNetCore.Mvc;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Domain.Models;

namespace pandaTeste.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceiroController : ControllerBase
    {
        private readonly IFinanceiroService _financeiroService;

        public FinanceiroController(IFinanceiroService financeiroService)
        {
            _financeiroService = financeiroService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Financeiro>>> ObterTodos()
        {
            try
            {
                var financeiros = await _financeiroService.ObterTodosAsync();
                return Ok(financeiros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Financeiro>> ObterPorId(int id)
        {
            try
            {
                var financeiro = await _financeiroService.ObterPorIdAsync(id);
                if (financeiro == null)
                    return NotFound("Financeiro não encontrado");

                return Ok(financeiro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("tipo/{tipo}")]
        public async Task<ActionResult<IEnumerable<Financeiro>>> ObterPorTipo(string tipo)
        {
            try
            {
                var financeiros = await _financeiroService.ObterPorTipoAsync(tipo);
                return Ok(financeiros);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("status/{baixado}")]
        public async Task<ActionResult<IEnumerable<Financeiro>>> ObterPorStatus(bool baixado)
        {
            try
            {
                var financeiros = await _financeiroService.ObterPorStatusAsync(baixado);
                return Ok(financeiros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("vencimentos")]
        public async Task<ActionResult<IEnumerable<Financeiro>>> ObterVencimentos([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            try
            {
                var financeiros = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);
                return Ok(financeiros);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Adicionar([FromBody] FinanceiroCreateDto dto)
        {
            try
            {
                await _financeiroService.AdicionarAsync(dto.Descricao, dto.Valor, dto.TipoFinanceiro, dto.DtVencimento);
                return Ok("Financeiro adicionado com sucesso");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPut("{id}/baixar")]
        public async Task<ActionResult> AlterarStatusBaixado(int id, [FromBody] BaixarDto dto)
        {
            try
            {
                var sucesso = await _financeiroService.AlterarStatusBaixadoAsync(id, dto.Baixado);
                if (!sucesso)
                    return NotFound("Financeiro não encontrado");

                return Ok("Status alterado com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPut("{id}/vencimento")]
        public async Task<ActionResult> AlterarDataVencimento(int id, [FromBody] VencimentoDto dto)
        {
            try
            {
                var sucesso = await _financeiroService.AlterarDataVencimentoAsync(id, dto.NovaDataVencimento);
                if (!sucesso)
                    return NotFound("Financeiro não encontrado");

                return Ok("Data de vencimento alterada com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Atualizar(int id, [FromBody] FinanceiroUpdateDto dto)
        {
            try
            {
                var sucesso = await _financeiroService.AtualizarAsync(id, dto.Descricao, dto.Valor, dto.TipoFinanceiro, dto.DtVencimento);
                if (!sucesso)
                    return NotFound("Financeiro não encontrado");

                return Ok("Financeiro atualizado com sucesso");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Remover(int id)
        {
            try
            {
                var sucesso = await _financeiroService.RemoverAsync(id);
                if (!sucesso)
                    return NotFound("Financeiro não encontrado");

                return Ok("Financeiro removido com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }

    // DTOs
    public class FinanceiroCreateDto
    {
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string TipoFinanceiro { get; set; }
        public DateTime DtVencimento { get; set; }
    }

    public class FinanceiroUpdateDto
    {
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string TipoFinanceiro { get; set; }
        public DateTime DtVencimento { get; set; }
    }

    public class BaixarDto
    {
        public bool Baixado { get; set; }
    }

    public class VencimentoDto
    {
        public DateTime NovaDataVencimento { get; set; }
    }
}
