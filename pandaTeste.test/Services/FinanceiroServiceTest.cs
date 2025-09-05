using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Models;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pandaTeste.Tests
{
    public class FinanceiroServiceTests
    {
        private readonly Mock<IFinanceiroRepository> _financeiroRepositoryMock;
        private readonly FinanceiroService _financeiroService;

        public FinanceiroServiceTests()
        {
            // Arrange: Inicializa os mocks e o serviço antes de cada teste.
            _financeiroRepositoryMock = new Mock<IFinanceiroRepository>();
            _financeiroService = new FinanceiroService(_financeiroRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaFinanceiro_QuandoIdExiste()
        {
            // Arrange
            var id = 1;
            var financeiro = new Financeiro { Id = id, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var result = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("Teste", result.Descricao);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaNull_QuandoIdNaoExiste()
        {
            // Arrange
            var id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaDeFinanceiros()
        {
            // Arrange
            var financeiros = new List<Financeiro> { new Financeiro { Id = 1 }, new Financeiro { Id = 2 } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task ObterPorTipoAsync_RetornaListaDeFinanceiros_QuandoTipoValido()
        {
            // Arrange
            var tipo = "Entrada";
            var financeiros = new List<Financeiro> { new Financeiro { Id = 1, TipoFinanceiro = tipo } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(tipo, result.First().TipoFinanceiro);
        }

        [Fact]
        public async Task ObterPorTipoAsync_LancaExcecao_QuandoTipoInvalido()
        {
            // Arrange
            var tipo = "Invalido";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterPorTipoAsync(tipo));
        }

        [Fact]
        public async Task ObterPorStatusAsync_RetornaListaDeFinanceiros_QuandoStatusExiste()
        {
            // Arrange
            var baixado = true;
            var financeiros = new List<Financeiro> { new Financeiro { Id = 1, Baixado = baixado } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(baixado)).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(baixado, result.First().Baixado);
        }

        [Fact]
        public async Task ObterVencimentosAsync_RetornaListaDeFinanceiros_QuandoDataValida()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddDays(-1);
            var dataFim = DateTime.Now.AddDays(1);
            var financeiros = new List<Financeiro> { new Financeiro { Id = 1, DtVencimento = DateTime.Now } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task ObterVencimentosAsync_LancaExcecao_QuandoDataInvalida()
        {
            // Arrange
            var dataInicio = DateTime.Now;
            var dataFim = DateTime.Now.AddDays(-1);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterVencimentosAsync(dataInicio, dataFim));
        }

        [Fact]
        public async Task AdicionarAsync_AdicionaFinanceiro_QuandoDadosValidos()
        {
            // Arrange
            var descricao = "Teste";
            var valor = 100;
            var tipoFinanceiro = "Entrada";
            var dtVencimento = DateTime.Now;

            // Act
            var result = await _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Theory]
        [InlineData("", 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 100, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AdicionarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            var dtVencimento = DateTime.Now;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_AtualizaStatus_QuandoIdExiste()
        {
            // Arrange
            var id = 1;
            var baixado = true;
            var financeiro = new Financeiro { Id = id, Baixado = !baixado };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var result = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_RetornaFalse_QuandoIdNaoExiste()
        {
            // Arrange
            var id = 1;
            var baixado = true;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_AtualizaData_QuandoIdExiste()
        {
            // Arrange
            var id = 1;
            var novaDataVencimento = DateTime.Now.AddDays(1);
            var financeiro = new Financeiro { Id = id, DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var result = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_RetornaFalse_QuandoIdNaoExiste()
        {
            // Arrange
            var id = 1;
            var novaDataVencimento = DateTime.Now.AddDays(1);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AtualizarAsync_AtualizaFinanceiro_QuandoDadosValidos()
        {
            // Arrange
            var id = 1;
            var descricao = "Teste";
            var valor = 100;
            var tipoFinanceiro = "Entrada";
            var dtVencimento = DateTime.Now;
            var financeiro = new Financeiro { Id = id };

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_RetornaFalse_QuandoIdNaoExiste()
        {
            // Arrange
            var id = 1;
            var descricao = "Teste";
            var valor = 100;
            var tipoFinanceiro = "Entrada";
            var dtVencimento = DateTime.Now;

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("", 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 100, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AtualizarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            var id = 1;
            var dtVencimento = DateTime.Now;
            var financeiro = new Financeiro { Id = id };

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task RemoverAsync_RemoveFinanceiro_QuandoIdExiste()
        {
            // Arrange
            var id = 1;
            var financeiro = new Financeiro { Id = id };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var result = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_RetornaFalse_QuandoIdNaoExiste()
        {
            // Arrange
            var id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.False(result);
        }
    }
}