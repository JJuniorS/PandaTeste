using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Application.Service;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Entities;
using Xunit;

namespace pandaTeste.api.Tests.Application.Service
{
    public class EstoqueServiceTests
    {
        private readonly Mock<IEstoqueRepository> _mockRepository;
        private readonly EstoqueService _estoqueService;

        public EstoqueServiceTests()
        {
            _mockRepository = new Mock<IEstoqueRepository>();
            _estoqueService = new EstoqueService(_mockRepository.Object);
        }

        #region AdicionarAoEstoque Tests

        [Fact]
        public void AdicionarAoEstoque_QuandoItemNaoExiste_DeveAdicionarNovoEstoque()
        {
            // Arrange
            var itemId = 1;
            var nomeItem = "Produto Teste";
            var quantidade = 10;

            _mockRepository.Setup(r => r.ObterPorItemId(itemId))
                          .Returns((Estoque)null);

            // Act
            _estoqueService.AdicionarAoEstoque(itemId, nomeItem, quantidade);

            // Assert
            _mockRepository.Verify(r => r.Adicionar(It.Is<Estoque>(e =>
                e.EstoqueItemId == itemId &&
                e.EstoqueItem.Id == itemId &&
                e.EstoqueItem.Nome == nomeItem &&
                e.QuantidadeEstoque == quantidade
            )), Times.Once);

            _mockRepository.Verify(r => r.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void AdicionarAoEstoque_QuandoItemJaExiste_DeveAtualizarQuantidade()
        {
            // Arrange
            var itemId = 1;
            var nomeItem = "Produto Teste";
            var quantidadeExistente = 5;
            var quantidadeAdicionar = 10;
            var quantidadeEsperada = quantidadeExistente + quantidadeAdicionar;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                EstoqueItem = new EstoqueItem { Id = itemId, Nome = nomeItem },
                QuantidadeEstoque = quantidadeExistente
            };

            _mockRepository.Setup(r => r.ObterPorItemId(itemId))
                          .Returns(estoqueExistente);

            // Act
            _estoqueService.AdicionarAoEstoque(itemId, nomeItem, quantidadeAdicionar);

            // Assert
            _mockRepository.Verify(r => r.Atualizar(It.Is<Estoque>(e =>
                e.Id == 1 &&
                e.EstoqueItemId == itemId &&
                e.QuantidadeEstoque == quantidadeEsperada
            )), Times.Once);

            _mockRepository.Verify(r => r.Adicionar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void AdicionarAoEstoque_ComQuantidadeZero_DeveAdicionarSemAlterarEstoque()
        {
            // Arrange
            var itemId = 1;
            var nomeItem = "Produto Teste";
            var quantidade = 0;

            _mockRepository.Setup(r => r.ObterPorItemId(itemId))
                          .Returns((Estoque)null);

            // Act
            _estoqueService.AdicionarAoEstoque(itemId, nomeItem, quantidade);

            // Assert
            _mockRepository.Verify(r => r.Adicionar(It.Is<Estoque>(e =>
                e.EstoqueItemId == itemId &&
                e.QuantidadeEstoque == 0
            )), Times.Once);
        }

        [Fact]
        public void AdicionarAoEstoque_ComQuantidadeNegativa_DevePermitirAdicao()
        {
            // Arrange
            var itemId = 1;
            var nomeItem = "Produto Teste";
            var quantidade = -5;

            _mockRepository.Setup(r => r.ObterPorItemId(itemId))
                          .Returns((Estoque)null);

            // Act
            _estoqueService.AdicionarAoEstoque(itemId, nomeItem, quantidade);

            // Assert
            _mockRepository.Verify(r => r.Adicionar(It.Is<Estoque>(e =>
                e.EstoqueItemId == itemId &&
                e.QuantidadeEstoque == quantidade
            )), Times.Once);
        }

        #endregion

        #region EntregarDoEstoque Tests

        [Fact]
        public void EntregarDoEstoque_QuandoTemEstoqueSuficiente_DeveRetornarTrueEAtualizarEstoque()
        {
            // Arrange
            var itemId = 1;
            var quantidadeEstoque = 10;
            var quantidadeEntrega = 5;
            var quantidadeEsperada = quantidadeEstoque - quantidadeEntrega;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                QuantidadeEstoque = quantidadeEstoque
            };

            _mockRepository.Setup(r => r.ObterPorItemId(itemId))
                          .Returns(estoqueExistente);

            // Act
            var resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntrega);

            // Assert
            Assert.True(resultado);
            Assert.Equal(quantidadeEsperada, estoqueExistente.QuantidadeEstoque);
            _mockRepository.Verify(r => r.Atualizar(estoqueExistente), Times.Once);
        }

        [Fact]
        public void EntregarDoEstoque_QuandoQuantidadeIgualAoEstoque_DeveRetornarTrueEZerarEstoque()
        {
            // Arrange
            var itemId = 1;
            var quantidadeEstoque = 10;
            var quantidadeEntrega = 10;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                QuantidadeEstoque = quantidadeEstoque
            };

            _mockRepository.Setup(r => r.ObterPorItemId(itemId))
                          .Returns(estoqueExistente);

            // Act
            var resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntrega);

            // Assert
            Assert.True(resultado);
            Assert.Equal(0, estoqueExistente.QuantidadeEstoque);
            _mockRepository.Verify(r => r.Atualizar(estoqueExistente), Times.Once);
        }

        [Fact]
        public void EntregarDoEstoque_QuandoNaoTemEstoqueSuficiente_DeveRetornarFalseESemAtualizacao()
        {
            // Arrange
            var itemId = 1;
            var quantidadeEstoque = 5;
            var quantidadeEntrega = 10;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                QuantidadeEstoque = quantidadeEstoque
            };

            _mockRepository.Setup(r => r.ObterPorItemId(itemId))
                          .Returns(estoqueExistente);

            // Act
            var resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntrega);

            // Assert
            Assert.False(resultado);
            Assert.Equal(quantidadeEstoque, estoqueExistente.QuantidadeEstoque); // Quantidade não deve alterar
            _mockRepository.Verify(r => r.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void EntregarDoEstoque_QuandoItemNaoExiste_DeveRetornarFalse()
        {
            // Arrange
            var itemId = 1;
            var quantidadeEntrega = 5;

            _mockRepository.Setup(r => r.ObterPorItemId(itemId))
                          .Returns((Estoque)null);

            // Act
            var resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntrega);

            // Assert
            Assert.False(resultado);
            _mockRepository.Verify(r => r.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }

        [Fact]
        public void EntregarDoEstoque_ComQuantidadeZero_DeveRetornarTrueESemAlterarEstoque()
        {
            // Arrange
            var itemId = 1;
            var quantidadeEstoque = 10;
            var quantidadeEntrega = 0;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                QuantidadeEstoque = quantidadeEstoque
            };

            _mockRepository.Setup(r => r.ObterPorItemId(itemId))
                          .Returns(estoqueExistente);

            // Act
            var resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntrega);

            // Assert
            Assert.True(resultado);
            Assert.Equal(quantidadeEstoque, estoqueExistente.QuantidadeEstoque);
            _mockRepository.Verify(r => r.Atualizar(estoqueExistente), Times.Once);
        }

        [Fact]
        public void EntregarDoEstoque_ComQuantidadeNegativa_DeveRetornarTrueEAumentarEstoque()
        {
            // Arrange
            var itemId = 1;
            var quantidadeEstoque = 10;
            var quantidadeEntrega = -5;
            var quantidadeEsperada = quantidadeEstoque - quantidadeEntrega; // 10 - (-5) = 15

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                QuantidadeEstoque = quantidadeEstoque
            };

            _mockRepository.Setup(r => r.ObterPorItemId(itemId))
                          .Returns(estoqueExistente);

            // Act
            var resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntrega);

            // Assert
            Assert.True(resultado);
            Assert.Equal(quantidadeEsperada, estoqueExistente.QuantidadeEstoque);
            _mockRepository.Verify(r => r.Atualizar(estoqueExistente), Times.Once);
        }

        [Fact]
        public void EntregarDoEstoque_QuandoEstoqueTemQuantidadeZero_DeveRetornarFalse()
        {
            // Arrange
            var itemId = 1;
            var quantidadeEstoque = 0;
            var quantidadeEntrega = 1;

            var estoqueExistente = new Estoque
            {
                Id = 1,
                EstoqueItemId = itemId,
                QuantidadeEstoque = quantidadeEstoque
            };

            _mockRepository.Setup(r => r.ObterPorItemId(itemId))
                          .Returns(estoqueExistente);

            // Act
            var resultado = _estoqueService.EntregarDoEstoque(itemId, quantidadeEntrega);

            // Assert
            Assert.False(resultado);
            Assert.Equal(0, estoqueExistente.QuantidadeEstoque);
            _mockRepository.Verify(r => r.Atualizar(It.IsAny<Estoque>()), Times.Never);
        }

        #endregion
    }
}