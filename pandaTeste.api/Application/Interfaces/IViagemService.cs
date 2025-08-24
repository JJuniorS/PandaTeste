using pandaTeste.api.Dtos;

namespace pandaTeste.api.Application.Interfaces
{
    public interface IViagemService
    {
        Task<List<Viagem>> ObterViagensAgendadasAsync();
    }
}
