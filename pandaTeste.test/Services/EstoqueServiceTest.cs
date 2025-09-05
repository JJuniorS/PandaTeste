using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Application.Service;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Entities;
using Xunit;

namespace PandaTeste.Tests.Application.Services
{
    public class EstoqueServiceTests
    {
        private readonly Mock<IEstoqueRepository> _mockEstoqueRepository;
        private readonly IEstoqueService _estoqueService;

        public EstoqueServiceTests()
        {
            // Arrange: Inicializa os mocks e o serviço antes de cada teste.
            _mockEstoqueRepository = new Mock<IEstoqueRepository>();
            _estoqueService = new EstoqueService(_mockEstoqueRepository.Object);
        }

        [Fact]
        public void AdicionarAoEstoque_ItemNaoExistente_DeveAdicionarNovoEstoque()
        {
            // Arrange: Configura o mock para retornar null quando o item não existe.
            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(It.IsAny<int>())).Returns((Estoque)null);

            // Act: Chama o método que estamos testando.
            _estoqueService.AdicionarAoEstoque(1, "Item Teste", 10);

            // Assert: Verifica se o método Adicionar foi chamado no repositório com os valores corretos.
            _mockEstoqueRepository.Verify(repo => repo.Adicionar(It.Is<Estoque>(e =>
                e.EstoqueItemId == 1 &&
                e.EstoqueItem.Nome == "Item Teste" &&
                e.QuantidadeEstoque == 10
            )), Times.Once);
        }

        [Fact]
        public void AdicionarAoEstoque_ItemExistente_DeveAtualizarEstoqueExistente()
        {
            // Arrange: Configura o mock para retornar um objeto Estoque existente.
            var estoqueExistente = new Estoque { Id = 1, EstoqueItemId = 1, EstoqueItem = new EstoqueItem { Id = 1, Nome = "Item Teste" }, QuantidadeEstoque = 5 };
            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(1)).Returns(estoqueExistente);

            // Act: Chama o método que estamos testando.
            _estoqueService.AdicionarAoEstoque(1, "Item Teste", 10);

            // Assert: Verifica se o método Atualizar foi chamado no repositório com a quantidade correta.
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.Is<Estoque>(e =>
                e.Id == 1 &&
                e.QuantidadeEstoque == 15
            )), Times.Once);
        }

        [Fact]
        public void EntregarDoEstoque_EstoqueSuficiente_DeveAtualizarEstoqueERetornarTrue()
        {
            // Arrange: Configura o mock para retornar um objeto Estoque com quantidade suficiente.
            var estoqueExistente = new Estoque { Id = 1, EstoqueItemId = 1, EstoqueItem = new EstoqueItem { Id = 1, Nome = "Item Teste" }, QuantidadeEstoque = 15 };
            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(1)).Returns(estoqueExistente);

            // Act: Chama o método que estamos testando.
            var resultado = _estoqueService.EntregarDoEstoque(1, 10);

            // Assert: Verifica se o método Atualizar foi chamado e se o resultado é true.
            Assert.True(resultado);
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.Is<Estoque>(e =>
                e.Id == 1 &&
                e.QuantidadeEstoque == 5
            )), Times.Once);
        }

        [Fact]
        public void EntregarDoEstoque_EstoqueInsuficiente_NaoDeveAtualizarEstoqueERetornarFalse()
        {
            // Arrange: Configura o mock para retornar um objeto Estoque com quantidade insuficiente.
            var estoqueExistente = new Estoque { Id = 1, EstoqueItemId = 1, EstoqueItem = new EstoqueItem { Id = 1, Nome = "Item Teste" }, QuantidadeEstoque = 5 };
            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(1)).Returns(estoqueExistente);

            // Act: Chama o método que estamos testando.
            var resultado = _estoqueService.EntregarDoEstoque(1, 10);

            // Assert: Verifica se o método Atualizar não foi chamado e se o resultado é false.
            Assert.False(resultado);
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void EntregarDoEstoque_ItemNaoExiste_NaoDeveAtualizarEstoqueERetornarFalse()
        {
            // Arrange: Configura o mock para retornar null quando o item não existe.
            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(It.IsAny<int>())).Returns((Estoque)null);

            // Act: Chama o método que estamos testando.
            var resultado = _estoqueService.EntregarDoEstoque(1, 10);

            // Assert: Verifica se o método Atualizar não foi chamado e se o resultado é false.
            Assert.False(resultado);
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }
    }
}