using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Entities;

namespace pandaTeste.api.Core.Repositories
{
    public class ViagemRepository : IViagemRepository
    {
        public async Task<List<Viagem>> ObterViagensAgendadasAsync()
        {
            // Simula uma operação assíncrona para obter os dados.
            var viagens = new List<Viagem>
            {
                new Viagem { Id = 1, Cliente = "João", DtViagem = System.DateTime.Now.AddDays(1), Destino = "Paris", Preco = 3500.50m, Orcamento = 4000.00m },
                new Viagem { Id = 2, Cliente = "Maria", DtViagem = System.DateTime.Now.AddDays(5), Destino = "Londres", Preco = 4200.00m, Orcamento = 4500.00m },
                new Viagem { Id = 3, Cliente = "Carlos", DtViagem = System.DateTime.Now.AddDays(10), Destino = "Nova York", Preco = 5000.75m, Orcamento = 5200.00m }
            };

            return await Task.FromResult(viagens);
        }
    }
}
