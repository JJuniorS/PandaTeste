using pandaTeste.api.Domain.Entities;

namespace pandaTeste.api.Core.Interfaces
{
    public interface IEstoqueRepository
    {
        void Adicionar(Estoque estoque);
        Estoque ObterPorItemId(int itemId);
        void Atualizar(Estoque estoque);
    }
}
