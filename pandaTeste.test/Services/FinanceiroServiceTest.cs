using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Models;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PandaTeste.Tests
{
    public class FinanceiroServiceTests
    {
        private readonly Mock<IFinanceiroRepository> _financeiroRepositoryMock;
        private readonly IFinanceiroService _financeiroService;

        public FinanceiroServiceTests()
        {
            // Arrange: Inicializa os mocks e o serviço antes de cada teste
            _financeiroRepositoryMock = new Mock<IFinanceiroRepository>();
            _financeiroService = new FinanceiroService(_financeiroRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaFinanceiro_QuandoIdExiste()
        {
            // Arrange
            int id = 1;
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
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var result = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaDeFinanceiros()
        {
            // Arrange
            var financeiros = new List<Financeiro> {
                new Financeiro { Id = 1, Descricao = "Teste1" },
                new Financeiro { Id = 2, Descricao = "Teste2" }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<Financeiro>>(result);
            Assert.Equal(2, ((List<Financeiro>)result).Count);
        }

        [Fact]
        public async Task ObterPorTipoAsync_RetornaListaDeFinanceiros_QuandoTipoValido()
        {
            // Arrange
            string tipo = "Entrada";
            var financeiros = new List<Financeiro> { new Financeiro { Id = 1, TipoFinanceiro = tipo } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<Financeiro>>(result);
            Assert.Single((List<Financeiro>)result);
            Assert.Equal(tipo, ((List<Financeiro>)result)[0].TipoFinanceiro);
        }

        [Fact]
        public async Task ObterPorTipoAsync_LancaExcecao_QuandoTipoInvalido()
        {
            // Arrange
            string tipo = "Invalido";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterPorTipoAsync(tipo));
        }

        [Fact]
        public async Task ObterPorStatusAsync_RetornaListaDeFinanceiros_QuandoStatusExiste()
        {
            // Arrange
            bool baixado = true;
            var financeiros = new List<Financeiro> { new Financeiro { Id = 1, Baixado = baixado } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(baixado)).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<Financeiro>>(result);
            Assert.Single((List<Financeiro>)result);
            Assert.Equal(baixado, ((List<Financeiro>)result)[0].Baixado);
        }

        [Fact]
        public async Task ObterVencimentosAsync_RetornaListaDeFinanceiros_QuandoDataValida()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now.AddDays(-1);
            DateTime dataFim = DateTime.Now.AddDays(1);
            var financeiros = new List<Financeiro> { new Financeiro { Id = 1, DtVencimento = DateTime.Now } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<Financeiro>>(result);
            Assert.Single((List<Financeiro>)result);
        }

        [Fact]
        public async Task ObterVencimentosAsync_LancaExcecao_QuandoDataInvalida()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now;
            DateTime dataFim = DateTime.Now.AddDays(-1);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterVencimentosAsync(dataInicio, dataFim));
        }

        [Fact]
        public async Task AdicionarAsync_RetornaTrue_QuandoDadosValidos()
        {
            // Arrange
            string descricao = "Teste";
            decimal valor = 10;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;
            _financeiroRepositoryMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Theory]
        [InlineData("", 10, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 10, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AdicionarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            DateTime dtVencimento = DateTime.Now;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_RetornaTrue_QuandoFinanceiroExiste()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            var financeiro = new Financeiro { Id = id, Baixado = !baixado };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.True(result);
            Assert.Equal(baixado, financeiro.Baixado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_RetornaFalse_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var result = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.False(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_RetornaTrue_QuandoFinanceiroExiste()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(10);
            var financeiro = new Financeiro { Id = id, DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.True(result);
            Assert.Equal(novaDataVencimento, financeiro.DtVencimento);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_RetornaFalse_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(10);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var result = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.False(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_RetornaTrue_QuandoDadosValidosEFincanceiroExiste()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste";
            decimal valor = 10;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            var financeiro = new Financeiro { Id = id };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(result);
            Assert.Equal(descricao, financeiro.Descricao);
            Assert.Equal(valor, financeiro.Valor);
            Assert.Equal(tipoFinanceiro, financeiro.TipoFinanceiro);
            Assert.Equal(dtVencimento, financeiro.DtVencimento);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_RetornaFalse_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste";
            decimal valor = 10;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.False(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Theory]
        [InlineData("", 10, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 10, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AtualizarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            int id = 1;
            DateTime dtVencimento = DateTime.Now;

            var financeiro = new Financeiro { Id = id };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);


            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task RemoverAsync_RetornaTrue_QuandoFinanceiroExiste()
        {
            // Arrange
            int id = 1;
            var financeiro = new Financeiro { Id = id };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);
            _financeiroRepositoryMock.Setup(repo => repo.RemoverAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_RetornaFalse_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var result = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.False(result);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Never);
        }
    }
}