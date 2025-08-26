using pandaTeste.api.Domain.Models;

namespace pandaTeste.api.Application.Interfaces
{
    public interface IFinanceiroService
    {
        Task<Financeiro> ObterPorIdAsync(int id);
        Task<IEnumerable<Financeiro>> ObterTodosAsync();
        Task<IEnumerable<Financeiro>> ObterPorTipoAsync(string tipo);
        Task<IEnumerable<Financeiro>> ObterPorStatusAsync(bool baixado);
        Task<IEnumerable<Financeiro>> ObterVencimentosAsync(DateTime dataInicio, DateTime dataFim);
        Task<bool> AdicionarAsync(string descricao, decimal valor, string tipoFinanceiro, DateTime dtVencimento);
        Task<bool> AlterarStatusBaixadoAsync(int id, bool baixado);
        Task<bool> AlterarDataVencimentoAsync(int id, DateTime novaDataVencimento);
        Task<bool> AtualizarAsync(int id, string descricao, decimal valor, string tipoFinanceiro, DateTime dtVencimento);
        Task<bool> RemoverAsync(int id);
    }
}
