using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Application.Service;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Entities;
using Xunit;

namespace pandaTeste.Tests.Application.Services
{
    public class EstoqueServiceTests
    {
        private readonly Mock<IEstoqueRepository> _estoqueRepositoryMock;
        private readonly IEstoqueService _estoqueService;

        public EstoqueServiceTests()
        {
            // Arrange: Inicializa os mocks e o serviço a ser testado.
            _estoqueRepositoryMock = new Mock<IEstoqueRepository>();
            _estoqueService = new EstoqueService(_estoqueRepositoryMock.Object);
        }

        [Fact]
        public void AdicionarAoEstoque_ItemNaoExistente_DeveAdicionarNovoEstoque()
        {
            // Arrange: Configura o mock para retornar null quando o item não existe.
            int itemId = 1;
            string nomeItem = "Produto A";
            int quantidade = 10;
            _estoqueRepositoryMock.Setup(repo => repo.ObterPorItemId(itemId)).Returns((Estoque)null);

            // Act: Chama o método a ser testado.
            _estoqueService.AdicionarAoEstoque(itemId, nomeItem, quantidade);

            // Assert: Verifica se o método Adicionar do repositório foi chamado com os valores corretos.
            _estoqueRepositoryMock.Verify(repo => repo.Adicionar(It.Is<Estoque>(e =>
                e.EstoqueItemId == itemId &&
                e.EstoqueItem.Nome == nomeItem &&
                e.QuantidadeEstoque == quantidade
            )), Times.Once);

            _estoqueRepositoryMock.Verify(repo => repo.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void AdicionarAoEstoque_ItemExistente_DeveAtualizarEstoqueExistente()
        {
            // Arrange: Configura o mock para retornar um item existente no estoque.
            int itemId = 1;
            string nomeItem = "Produto A";
            int quantidadeAdicionada = 10;
            var estoqueExistente = new Estoque { Id = 1, EstoqueItemId = itemId, EstoqueItem = new EstoqueItem { Id = itemId, Nome = nomeItem }, QuantidadeEstoque = 5 };
            _estoqueRepositoryMock.Setup(repo => repo.ObterPorItemId(itemId)).Returns(estoqueExistente);

            // Act: Chama o método a ser testado.
            _estoqueService.AdicionarAoEstoque(itemId, nomeItem, quantidadeAdicionada);

            // Assert: Verifica se o método Atualizar do repositório foi chamado com a quantidade correta.
            _estoqueRepositoryMock.Verify(repo => repo.Atualizar(It.Is<Estoque>(e =>
                e.EstoqueItemId == itemId &&
                e.QuantidadeEstoque == 15
            )), Times.Once);

            _estoqueRepositoryMock.Verify(repo => repo.Adicionar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void EntregarDoEstoque_QuantidadeSuficiente_DeveAtualizarEstoqueERetornarTrue()
        {
            // Arrange: Configura o mock para retornar um item existente com quantidade suficiente.
            int itemId = 1;
            int quantidadeEntregue = 5;
            var estoqueExistente = new Estoque { Id = 1, EstoqueItemId = itemId, QuantidadeEstoque = 10 };
            _estoqueRepositoryMock.Setup(repo => repo.ObterPorItemId(itemId)).Returns(estoqueExistente);

            // Act: Chama o método a ser testado.
            var resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntregue);

            // Assert: Verifica se o método Atualizar do repositório foi chamado com a quantidade correta e se o resultado é verdadeiro.
            Assert.True(resultado);
            _estoqueRepositoryMock.Verify(repo => repo.Atualizar(It.Is<Estoque>(e =>
                e.EstoqueItemId == itemId &&
                e.QuantidadeEstoque == 5
            )), Times.Once);
        }

        [Fact]
        public void EntregarDoEstoque_QuantidadeInsuficiente_NaoDeveAtualizarEstoqueERetornarFalse()
        {
            // Arrange: Configura o mock para retornar um item existente com quantidade insuficiente.
            int itemId = 1;
            int quantidadeEntregue = 15;
            var estoqueExistente = new Estoque { Id = 1, EstoqueItemId = itemId, QuantidadeEstoque = 10 };
            _estoqueRepositoryMock.Setup(repo => repo.ObterPorItemId(itemId)).Returns(estoqueExistente);

            // Act: Chama o método a ser testado.
            var resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntregue);

            // Assert: Verifica se o método Atualizar do repositório não foi chamado e se o resultado é falso.
            Assert.False(resultado);
            _estoqueRepositoryMock.Verify(repo => repo.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void EntregarDoEstoque_ItemNaoExistente_NaoDeveAtualizarEstoqueERetornarFalse()
        {
            // Arrange: Configura o mock para retornar null, indicando que o item não existe.
            int itemId = 1;
            int quantidadeEntregue = 5;
            _estoqueRepositoryMock.Setup(repo => repo.ObterPorItemId(itemId)).Returns((Estoque)null);

            // Act: Chama o método a ser testado.
            var resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntregue);

            // Assert: Verifica se o método Atualizar do repositório não foi chamado e se o resultado é falso.
            Assert.False(resultado);
            _estoqueRepositoryMock.Verify(repo => repo.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }
    }
}