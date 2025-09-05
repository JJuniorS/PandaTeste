using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Application.Service;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Entities;
using Xunit;

namespace pandaTeste.Tests
{
    public class EstoqueServiceTests
    {
        private readonly Mock<IEstoqueRepository> _mockEstoqueRepository;
        private readonly IEstoqueService _estoqueService;

        public EstoqueServiceTests()
        {
            // Arrange: Inicializa os mocks e o serviço a ser testado no construtor para reutilização.
            _mockEstoqueRepository = new Mock<IEstoqueRepository>();
            _estoqueService = new EstoqueService(_mockEstoqueRepository.Object);
        }

        [Fact]
        public void AdicionarAoEstoque_ItemNaoExistente_DeveAdicionarNovoEstoque()
        {
            // Arrange: Configura o mock para retornar null quando o item não existe.
            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(It.IsAny<int>())).Returns((Estoque)null);
            _mockEstoqueRepository.Setup(repo => repo.Adicionar(It.IsAny<Estoque>()));

            int itemId = 1;
            string nomeItem = "Produto A";
            int quantidade = 10;

            // Act: Chama o método a ser testado.
            _estoqueService.AdicionarAoEstoque(itemId, nomeItem, quantidade);

            // Assert: Verifica se o método Adicionar foi chamado no repositório com os valores corretos.
            _mockEstoqueRepository.Verify(repo => repo.Adicionar(It.Is<Estoque>(e =>
                e.EstoqueItemId == itemId &&
                e.EstoqueItem.Nome == nomeItem &&
                e.QuantidadeEstoque == quantidade
            )), Times.Once);
        }

        [Fact]
        public void AdicionarAoEstoque_ItemExistente_DeveAtualizarEstoque()
        {
            // Arrange: Configura o mock para retornar um item existente.
            int itemId = 1;
            string nomeItem = "Produto A";
            int quantidadeInicial = 5;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                EstoqueItem = new EstoqueItem { Id = itemId, Nome = nomeItem },
                QuantidadeEstoque = quantidadeInicial
            };

            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(itemId)).Returns(estoqueExistente);
            _mockEstoqueRepository.Setup(repo => repo.Atualizar(It.IsAny<Estoque>()));

            int quantidadeAdicionada = 10;

            // Act: Chama o método a ser testado.
            _estoqueService.AdicionarAoEstoque(itemId, nomeItem, quantidadeAdicionada);

            // Assert: Verifica se o método Atualizar foi chamado no repositório com a quantidade correta.
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.Is<Estoque>(e =>
                e.EstoqueItemId == itemId &&
                e.EstoqueItem.Nome == nomeItem &&
                e.QuantidadeEstoque == quantidadeInicial + quantidadeAdicionada
            )), Times.Once);

            _mockEstoqueRepository.Verify(repo => repo.Adicionar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void EntregarDoEstoque_EstoqueSuficiente_DeveAtualizarEstoqueERetornarTrue()
        {
            // Arrange: Configura o mock para retornar um item com estoque suficiente.
            int itemId = 1;
            int quantidadeInicial = 15;
            int quantidadeEntregue = 10;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                EstoqueItem = new EstoqueItem { Id = itemId, Nome = "Produto A" },
                QuantidadeEstoque = quantidadeInicial
            };

            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(itemId)).Returns(estoqueExistente);
            _mockEstoqueRepository.Setup(repo => repo.Atualizar(It.IsAny<Estoque>()));

            // Act: Chama o método a ser testado.
            bool resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntregue);

            // Assert: Verifica se o método Atualizar foi chamado e se o resultado é true.
            Assert.True(resultado);
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.Is<Estoque>(e =>
                e.EstoqueItemId == itemId &&
                e.QuantidadeEstoque == quantidadeInicial - quantidadeEntregue
            )), Times.Once);
        }

        [Fact]
        public void EntregarDoEstoque_EstoqueInsuficiente_NaoDeveAtualizarEstoqueERetornarFalse()
        {
            // Arrange: Configura o mock para retornar um item com estoque insuficiente.
            int itemId = 1;
            int quantidadeInicial = 5;
            int quantidadeEntregue = 10;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                EstoqueItem = new EstoqueItem { Id = itemId, Nome = "Produto A" },
                QuantidadeEstoque = quantidadeInicial
            };

            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(itemId)).Returns(estoqueExistente);

            // Act: Chama o método a ser testado.
            bool resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntregue);

            // Assert: Verifica se o método Atualizar não foi chamado e se o resultado é false.
            Assert.False(resultado);
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void EntregarDoEstoque_ItemNaoExistente_NaoDeveAtualizarEstoqueERetornarFalse()
        {
            // Arrange: Configura o mock para retornar null quando o item não existe.
            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(It.IsAny<int>())).Returns((Estoque)null);

            int itemId = 1;
            int quantidadeEntregue = 10;

            // Act: Chama o método a ser testado.
            bool resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntregue);

            // Assert: Verifica se o método Atualizar não foi chamado e se o resultado é false.
            Assert.False(resultado);
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }
    }
}