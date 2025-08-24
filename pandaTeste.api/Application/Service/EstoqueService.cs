using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Entities;

namespace pandaTeste.api.Application.Service
{
    public class EstoqueService : IEstoqueService
    {
        private readonly IEstoqueRepository _repository;

        public EstoqueService(IEstoqueRepository repository)
        {
            _repository = repository;
        }

        public void AdicionarAoEstoque(int itemId, string nomeItem, int quantidade)
        {
            var estoque = _repository.ObterPorItemId(itemId) ?? new Estoque
            {
                EstoqueItemId = itemId,
                EstoqueItem = new EstoqueItem { Id = itemId, Nome = nomeItem },
                QuantidadeEstoque = 0
            };

            estoque.QuantidadeEstoque += quantidade;

            if (estoque.Id == 0)
                _repository.Adicionar(estoque);
            else
                _repository.Atualizar(estoque);
        }

        public bool EntregarDoEstoque(int itemId, int quantidade)
        {
            var estoque = _repository.ObterPorItemId(itemId);
            if (estoque == null || estoque.QuantidadeEstoque < quantidade)
                return false;

            estoque.QuantidadeEstoque -= quantidade;
            _repository.Atualizar(estoque);
            return true;
        }
    }
}
