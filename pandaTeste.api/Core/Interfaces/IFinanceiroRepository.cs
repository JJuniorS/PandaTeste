using pandaTeste.api.Domain.Models;

namespace pandaTeste.api.Core.Interfaces
{
    public interface IFinanceiroRepository
    {
        Task<Financeiro> ObterPorIdAsync(int id);
        Task<IEnumerable<Financeiro>> ObterTodosAsync();
        Task<IEnumerable<Financeiro>> ObterPorTipoAsync(string tipo);
        Task<IEnumerable<Financeiro>> ObterPorStatusAsync(bool baixado);
        Task<IEnumerable<Financeiro>> ObterVencimentosAsync(DateTime dataInicio, DateTime dataFim);
        Task AdicionarAsync(Financeiro financeiro);
        Task AtualizarAsync(Financeiro financeiro);
        Task RemoverAsync(int id);
    }
}
