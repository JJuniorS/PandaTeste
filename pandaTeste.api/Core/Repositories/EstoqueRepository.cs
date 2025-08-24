using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Entities;
using pandaTeste.api.Infrastructure.Context;

namespace pandaTeste.api.Core.Repositories
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly List<Estoque> _dados = new();
        public EstoqueRepository()
        {
            // Dados iniciais para testes
            _dados = new List<Estoque>
            {
                new Estoque
                {
                    Id = 1,
                    QuantidadeEstoque = 50,
                    EstoqueItemId = 1,
                    EstoqueItem = new EstoqueItem { Id = 1, Nome = "Teclado Mecânico" }
                },
                new Estoque
                {
                    Id = 2,
                    QuantidadeEstoque = 20,
                    EstoqueItemId = 2,
                    EstoqueItem = new EstoqueItem { Id = 2, Nome = "Mouse Gamer" }
                },
                new Estoque
                {
                    Id = 3,
                    QuantidadeEstoque = 10,
                    EstoqueItemId = 3,
                    EstoqueItem = new EstoqueItem { Id = 3, Nome = "Monitor 27''" }
                }
            };
        }
        public void Adicionar(Estoque estoque)
        {
            estoque.Id = _dados.Any() ? _dados.Max(e => e.Id) + 1 : 1;
            _dados.Add(estoque);
        }

        public Estoque ObterPorItemId(int itemId)
        {
            return _dados.FirstOrDefault(e => e.EstoqueItemId == itemId);
        }

        public void Atualizar(Estoque estoque)
        {
            var exist = _dados.FirstOrDefault(e => e.Id == estoque.Id);
            if (exist != null)
            {
                exist.QuantidadeEstoque = estoque.QuantidadeEstoque;
            }
        }
    }
}
