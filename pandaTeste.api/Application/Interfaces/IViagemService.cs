using pandaTeste.api.Domain.Entities;

namespace pandaTeste.api.Application.Interfaces
{
    public interface IViagemService
    {
        Task<List<Viagem>> ObterViagensAgendadasAsync();
    }
}
