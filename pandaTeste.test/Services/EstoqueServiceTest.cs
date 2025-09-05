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
            // Arrange: Inicializa os mocks e o serviço a ser testado.
            _mockEstoqueRepository = new Mock<IEstoqueRepository>();
            _estoqueService = new EstoqueService(_mockEstoqueRepository.Object);
        }

        [Fact]
        public void AdicionarAoEstoque_ItemNaoExistente_DeveAdicionarNovoItemAoEstoque()
        {
            // Arrange: Define os parâmetros para o método e configura o mock do repositório.
            int itemId = 1;
            string nomeItem = "Produto A";
            int quantidade = 10;

            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(itemId)).Returns((Estoque)null);

            // Act: Executa o método a ser testado.
            _estoqueService.AdicionarAoEstoque(itemId, nomeItem, quantidade);

            // Assert: Verifica se o método Adicionar do repositório foi chamado com os parâmetros corretos.
            _mockEstoqueRepository.Verify(repo => repo.Adicionar(It.Is<Estoque>(e =>
                e.EstoqueItemId == itemId &&
                e.EstoqueItem.Nome == nomeItem &&
                e.QuantidadeEstoque == quantidade
            )), Times.Once);

            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void AdicionarAoEstoque_ItemExistente_DeveAtualizarQuantidadeNoEstoque()
        {
            // Arrange: Define os parâmetros e configura o mock para retornar um item existente.
            int itemId = 1;
            string nomeItem = "Produto A";
            int quantidadeAdicionada = 5;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                EstoqueItem = new EstoqueItem { Id = itemId, Nome = nomeItem },
                QuantidadeEstoque = 10
            };

            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(itemId)).Returns(estoqueExistente);

            // Act: Executa o método a ser testado.
            _estoqueService.AdicionarAoEstoque(itemId, nomeItem, quantidadeAdicionada);

            // Assert: Verifica se o método Atualizar do repositório foi chamado com a quantidade correta.
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.Is<Estoque>(e =>
                e.EstoqueItemId == itemId &&
                e.EstoqueItem.Nome == nomeItem &&
                e.QuantidadeEstoque == 15 // 10 (original) + 5 (adicionado)
            )), Times.Once);

            _mockEstoqueRepository.Verify(repo => repo.Adicionar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void EntregarDoEstoque_QuantidadeSuficiente_DeveAtualizarEstoqueERetornarTrue()
        {
            // Arrange: Configura o mock para simular um estoque com quantidade suficiente.
            int itemId = 1;
            int quantidadeSolicitada = 5;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                EstoqueItem = new EstoqueItem { Id = itemId, Nome = "Produto A" },
                QuantidadeEstoque = 10
            };

            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(itemId)).Returns(estoqueExistente);

            // Act: Executa o método a ser testado.
            bool resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeSolicitada);

            // Assert: Verifica se o método retorna true e se a quantidade no estoque foi atualizada corretamente.
            Assert.True(resultado);
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.Is<Estoque>(e => e.QuantidadeEstoque == 5)), Times.Once); //10 - 5 = 5
        }

        [Fact]
        public void EntregarDoEstoque_QuantidadeInsuficiente_NaoDeveAtualizarEstoqueERetornarFalse()
        {
            // Arrange: Configura o mock para simular um estoque com quantidade insuficiente.
            int itemId = 1;
            int quantidadeSolicitada = 15;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                EstoqueItem = new EstoqueItem { Id = itemId, Nome = "Produto A" },
                QuantidadeEstoque = 10
            };

            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(itemId)).Returns(estoqueExistente);

            // Act: Executa o método a ser testado.
            bool resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeSolicitada);

            // Assert: Verifica se o método retorna false e se o método Atualizar não foi chamado.
            Assert.False(resultado);
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void EntregarDoEstoque_ItemNaoExistente_NaoDeveAtualizarEstoqueERetornarFalse()
        {
            // Arrange: Configura o mock para simular que o item não existe no estoque.
            int itemId = 1;
            int quantidadeSolicitada = 5;

            _mockEstoqueRepository.Setup(repo => repo.ObterPorItemId(itemId)).Returns((Estoque)null);

            // Act: Executa o método a ser testado.
            bool resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeSolicitada);

            // Assert: Verifica se o método retorna false e se o método Atualizar não foi chamado.
            Assert.False(resultado);
            _mockEstoqueRepository.Verify(repo => repo.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }
    }
}