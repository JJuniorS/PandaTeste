using pandaTeste.api.Dtos;

namespace pandaTeste.api.Core.Interfaces
{
    public interface IViagemRepository
    {
        Task<List<Viagem>> ObterViagensAgendadasAsync();
    }
}
