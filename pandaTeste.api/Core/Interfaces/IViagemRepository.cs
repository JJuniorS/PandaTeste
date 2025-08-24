using pandaTeste.api.Domain.Entities;

namespace pandaTeste.api.Core.Interfaces
{
    public interface IViagemRepository
    {
        Task<List<Viagem>> ObterViagensAgendadasAsync();
    }
}
