using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Models;

public class FinanceiroService : IFinanceiroService
{
    private readonly IFinanceiroRepository _repository;
    private readonly List<string> _tiposPermitidos = new List<string> { "Entrada", "Saída" };

    public FinanceiroService(IFinanceiroRepository repository)
    {
        _repository = repository;
    }

    public async Task<Financeiro> ObterPorIdAsync(int id)
    {
        return await _repository.ObterPorIdAsync(id);
    }

    public async Task<IEnumerable<Financeiro>> ObterTodosAsync()
    {
        return await _repository.ObterTodosAsync();
    }

    public async Task<IEnumerable<Financeiro>> ObterPorTipoAsync(string tipo)
    {
        if (!_tiposPermitidos.Contains(tipo))
            throw new ArgumentException($"Tipo financeiro inválido. Use: {string.Join(", ", _tiposPermitidos)}");

        return await _repository.ObterPorTipoAsync(tipo);
    }

    public async Task<IEnumerable<Financeiro>> ObterPorStatusAsync(bool baixado)
    {
        return await _repository.ObterPorStatusAsync(baixado);
    }

    public async Task<IEnumerable<Financeiro>> ObterVencimentosAsync(DateTime dataInicio, DateTime dataFim)
    {
        if (dataInicio > dataFim)
            throw new ArgumentException("Data início não pode ser maior que data fim");

        return await _repository.ObterVencimentosAsync(dataInicio, dataFim);
    }

    public async Task<bool> AdicionarAsync(string descricao, decimal valor, string tipoFinanceiro, DateTime dtVencimento)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória");

        if (valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero");

        if (!_tiposPermitidos.Contains(tipoFinanceiro))
            throw new ArgumentException($"Tipo financeiro inválido. Use: {string.Join(", ", _tiposPermitidos)}");

        var financeiro = new Financeiro
        {
            Descricao = descricao.Trim(),
            Valor = valor,
            TipoFinanceiro = tipoFinanceiro,
            DtVencimento = dtVencimento,
            Baixado = false,
            DtCadastro = DateTime.Now
        };

        await _repository.AdicionarAsync(financeiro);
        return true;
    }

    public async Task<bool> AlterarStatusBaixadoAsync(int id, bool baixado)
    {
        var financeiro = await _repository.ObterPorIdAsync(id);
        if (financeiro == null)
            return false;

        financeiro.Baixado = baixado;
        financeiro.DtBaixa = baixado ? DateTime.Now : null;

        await _repository.AtualizarAsync(financeiro);
        return true;
    }

    public async Task<bool> AlterarDataVencimentoAsync(int id, DateTime novaDataVencimento)
    {
        var financeiro = await _repository.ObterPorIdAsync(id);
        if (financeiro == null)
            return false;

        financeiro.DtVencimento = novaDataVencimento;
        await _repository.AtualizarAsync(financeiro);
        return true;
    }

    public async Task<bool> AtualizarAsync(int id, string descricao, decimal valor, string tipoFinanceiro, DateTime dtVencimento)
    {
        var financeiro = await _repository.ObterPorIdAsync(id);
        if (financeiro == null)
            return false;

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória");

        if (valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero");

        if (!_tiposPermitidos.Contains(tipoFinanceiro))
            throw new ArgumentException($"Tipo financeiro inválido. Use: {string.Join(", ", _tiposPermitidos)}");

        financeiro.Descricao = descricao.Trim();
        financeiro.Valor = valor;
        financeiro.TipoFinanceiro = tipoFinanceiro;
        financeiro.DtVencimento = dtVencimento;

        await _repository.AtualizarAsync(financeiro);
        return true;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var financeiro = await _repository.ObterPorIdAsync(id);
        if (financeiro == null)
            return false;

        await _repository.RemoverAsync(id);
        return true;
    }
}
