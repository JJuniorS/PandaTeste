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
        private readonly IFinanceiroService _financeiroService;

        public FinanceiroServiceTests()
        {
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
            Assert.Equal(financeiro, result);
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
            var financeiros = new List<Financeiro> { new Financeiro { Descricao = "Teste1" }, new Financeiro { Descricao = "Teste2" } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.Equal(financeiros, result);
        }

        [Fact]
        public async Task ObterPorTipoAsync_RetornaFinanceirosDoTipo_QuandoTipoValido()
        {
            // Arrange
            var tipo = "Entrada";
            var financeiros = new List<Financeiro> { new Financeiro { TipoFinanceiro = tipo, Descricao = "Teste1" } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.Equal(financeiros, result);
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
        public async Task ObterPorStatusAsync_RetornaFinanceirosComStatusCorreto()
        {
            // Arrange
            var baixado = true;
            var financeiros = new List<Financeiro> { new Financeiro { Baixado = baixado, Descricao = "Teste1" } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(baixado)).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.Equal(financeiros, result);
        }

        [Fact]
        public async Task ObterVencimentosAsync_RetornaFinanceirosNoPeriodo()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddDays(-1);
            var dataFim = DateTime.Now.AddDays(1);
            var financeiros = new List<Financeiro> { new Financeiro { DtVencimento = DateTime.Now, Descricao = "Teste1" } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeiros);

            // Act
            var result = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.Equal(financeiros, result);
        }

        [Fact]
        public async Task ObterVencimentosAsync_LancaExcecao_QuandoDataInicioMaiorQueDataFim()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddDays(1);
            var dataFim = DateTime.Now.AddDays(-1);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterVencimentosAsync(dataInicio, dataFim));
        }

        [Fact]
        public async Task AdicionarAsync_AdicionaFinanceiro_QuandoDadosValidos()
        {
            // Arrange
            var descricao = "Teste";
            var valor = 10;
            var tipoFinanceiro = "Entrada";
            var dtVencimento = DateTime.Now;

            // Act
            var result = await _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.Is<Financeiro>(f =>
                f.Descricao == descricao &&
                f.Valor == valor &&
                f.TipoFinanceiro == tipoFinanceiro &&
                f.DtVencimento == dtVencimento
            )), Times.Once);
        }

        [Theory]
        [InlineData("", 10, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 10, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AdicionarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemEsperada)
        {
            // Arrange
            var dtVencimento = DateTime.Now;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemEsperada, exception.Message);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_AtualizaStatus_QuandoFinanceiroExiste()
        {
            // Arrange
            var id = 1;
            var baixado = true;
            var financeiro = new Financeiro { Id = id, Baixado = false };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var result = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.True(result);
            Assert.Equal(baixado, financeiro.Baixado);
            Assert.NotNull(financeiro.DtBaixa);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiro), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_NaoFazNada_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            var baixado = true;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.False(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_AtualizaData_QuandoFinanceiroExiste()
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
            Assert.Equal(novaDataVencimento, financeiro.DtVencimento);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiro), Times.Once);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_NaoFazNada_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            var novaDataVencimento = DateTime.Now.AddDays(1);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.False(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }


        [Fact]
        public async Task AtualizarAsync_AtualizaFinanceiro_QuandoDadosValidos()
        {
            // Arrange
            var id = 1;
            var descricao = "Teste Atualizado";
            var valor = 20;
            var tipoFinanceiro = "Saída";
            var dtVencimento = DateTime.Now.AddDays(2);
            var financeiroExistente = new Financeiro { Id = id, Descricao = "Teste", Valor = 10, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroExistente);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(result);
            Assert.Equal(descricao, financeiroExistente.Descricao);
            Assert.Equal(valor, financeiroExistente.Valor);
            Assert.Equal(tipoFinanceiro, financeiroExistente.TipoFinanceiro);
            Assert.Equal(dtVencimento, financeiroExistente.DtVencimento);

            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiroExistente), Times.Once);
        }


        [Theory]
        [InlineData("", 20, "Saída", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Saída", "Valor deve ser maior que zero")]
        [InlineData("Teste", 20, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AtualizarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemEsperada)
        {
            // Arrange
            var id = 1;
            var dtVencimento = DateTime.Now;
            var financeiroExistente = new Financeiro { Id = id, Descricao = "Teste", Valor = 10, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroExistente);


            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemEsperada, exception.Message);
        }


        [Fact]
        public async Task AtualizarAsync_RetornaFalso_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, "Teste", 10, "Entrada", DateTime.Now);

            // Assert
            Assert.False(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);

        }

        [Fact]
        public async Task RemoverAsync_RemoveFinanceiro_QuandoFinanceiroExiste()
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
        public async Task RemoverAsync_NaoFazNada_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.False(result);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(It.IsAny<int>()), Times.Never);
        }


    }
}