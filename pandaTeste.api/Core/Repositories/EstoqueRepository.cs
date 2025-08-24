using pandaTeste.api.Domain.Entities;
using pandaTeste.api.Infrastructure.Context;

namespace pandaTeste.api.Core.Repositories
{
    public class EstoqueRepository
    {
        private readonly PandaDbContext _context;

        public EstoqueRepository(PandaDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarItemEstoque(EstoqueItem item)
        {
            _context.EstoqueItens.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task EntregarItemEstoque(int estoqueId, int quantidade)
        {
            var estoque = await _context.Estoques.FindAsync(estoqueId);
            if (estoque != null)
            {
                estoque.QuantidadeEstoque -= quantidade;
                await _context.SaveChangesAsync();
            }
        }
    }
}
