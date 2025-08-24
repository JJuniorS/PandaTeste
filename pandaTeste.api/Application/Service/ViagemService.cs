using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Entities;

namespace pandaTeste.api.Application.Service
{
    public class ViagemService : IViagemService
    {
        public IViagemRepository _viagemRepository;
        public ViagemService(IViagemRepository viagemRepository)
        {
            _viagemRepository = viagemRepository;
        }

        /// <summary>
        /// Obtém uma lista de viagens agendadas do repositório.
        /// </summary>
        /// <returns>Uma lista de objetos Viagem.</returns>
        public async Task<List<Viagem>> ObterViagensAgendadasAsync()
        {
            // A lógica de negócio reside aqui. Por exemplo, você pode adicionar
            // validações, regras, ou qualquer outra operação antes de chamar o repositório.
            // Por enquanto, apenas chamamos o repositório para obter os dados.
            return await _viagemRepository.ObterViagensAgendadasAsync();
        }
    }
}
