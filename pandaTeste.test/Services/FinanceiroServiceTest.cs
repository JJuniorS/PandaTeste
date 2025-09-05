using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace pandaTeste.Tests
{
    public class FinanceiroServiceTests
    {
        private readonly Mock<IFinanceiroRepository> _financeiroRepositoryMock;
        private readonly FinanceiroService _financeiroService;

        public FinanceiroServiceTests()
        {
            // Arrange: Inicializa os mocks e o serviço a ser testado.
            _financeiroRepositoryMock = new Mock<IFinanceiroRepository>();
            _financeiroService = new FinanceiroService(_financeiroRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaFinanceiro_QuandoIdExiste()
        {
            // Arrange
            int id = 1;
            var financeiroEsperado = new Financeiro { Id = id, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroEsperado);

            // Act
            var financeiroRetornado = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Equal(financeiroEsperado, financeiroRetornado);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaNull_QuandoIdNaoExiste()
        {
            // Arrange
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var financeiroRetornado = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Null(financeiroRetornado);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaDeFinanceiros()
        {
            // Arrange
            var financeirosEsperados = new List<Financeiro> { new Financeiro { Descricao = "Teste1" }, new Financeiro { Descricao = "Teste2" } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(financeirosEsperados);

            // Act
            var financeirosRetornados = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.Equal(financeirosEsperados, financeirosRetornados);
        }

        [Fact]
        public async Task ObterPorTipoAsync_RetornaListaDeFinanceiros_QuandoTipoValido()
        {
            // Arrange
            string tipo = "Entrada";
            var financeirosEsperados = new List<Financeiro> { new Financeiro { TipoFinanceiro = tipo } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(financeirosEsperados);

            // Act
            var financeirosRetornados = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.Equal(financeirosEsperados, financeirosRetornados);
        }

        [Fact]
        public async Task ObterPorTipoAsync_LancaExcecao_QuandoTipoInvalido()
        {
            // Arrange
            string tipoInvalido = "Invalido";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterPorTipoAsync(tipoInvalido));
        }

        [Fact]
        public async Task ObterPorStatusAsync_RetornaListaDeFinanceiros_QuandoStatusExiste()
        {
            // Arrange
            bool baixado = true;
            var financeirosEsperados = new List<Financeiro> { new Financeiro { Baixado = baixado } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(baixado)).ReturnsAsync(financeirosEsperados);

            // Act
            var financeirosRetornados = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.Equal(financeirosEsperados, financeirosRetornados);
        }

        [Fact]
        public async Task ObterVencimentosAsync_RetornaListaDeFinanceiros_QuandoDataInicioMenorOuIgualDataFim()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now.AddDays(-1);
            DateTime dataFim = DateTime.Now;
            var financeirosEsperados = new List<Financeiro> { new Financeiro { DtVencimento = dataFim } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeirosEsperados);

            // Act
            var financeirosRetornados = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.Equal(financeirosEsperados, financeirosRetornados);
        }

        [Fact]
        public async Task ObterVencimentosAsync_LancaExcecao_QuandoDataInicioMaiorQueDataFim()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now;
            DateTime dataFim = DateTime.Now.AddDays(-1);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterVencimentosAsync(dataInicio, dataFim));
        }

        [Fact]
        public async Task AdicionarAsync_AdicionaFinanceiro_QuandoDadosValidos()
        {
            // Arrange
            string descricao = "Teste";
            decimal valor = 100;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            // Act
            var resultado = await _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.Is<Financeiro>(f =>
                f.Descricao == descricao && f.Valor == valor && f.TipoFinanceiro == tipoFinanceiro && f.DtVencimento == dtVencimento)), Times.Once);
        }

        [Theory]
        [InlineData(null, 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("", 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", -100, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 100, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AdicionarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            DateTime dtVencimento = DateTime.Now;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_AtualizaStatus_QuandoFinanceiroExiste()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            var financeiro = new Financeiro { Id = id, Baixado = false };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.True(resultado);
            Assert.Equal(baixado, financeiro.Baixado);
            Assert.NotNull(financeiro.DtBaixa);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiro), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_RetornaFalso_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_AtualizaDataVencimento_QuandoFinanceiroExiste()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(7);
            var financeiro = new Financeiro { Id = id, DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.True(resultado);
            Assert.Equal(novaDataVencimento, financeiro.DtVencimento);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiro), Times.Once);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_RetornaFalso_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(7);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task AtualizarAsync_AtualizaFinanceiro_QuandoDadosValidos()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste Atualizado";
            decimal valor = 200;
            string tipoFinanceiro = "Saída";
            DateTime dtVencimento = DateTime.Now.AddDays(10);
            var financeiroExistente = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroExistente);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);
            Assert.Equal(descricao, financeiroExistente.Descricao);
            Assert.Equal(valor, financeiroExistente.Valor);
            Assert.Equal(tipoFinanceiro, financeiroExistente.TipoFinanceiro);
            Assert.Equal(dtVencimento, financeiroExistente.DtVencimento);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiroExistente), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_RetornaFalso_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste Atualizado";
            decimal valor = 200;
            string tipoFinanceiro = "Saída";
            DateTime dtVencimento = DateTime.Now.AddDays(10);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.False(resultado);
        }

        [Theory]
        [InlineData(null, 200, "Saída", "Descrição é obrigatória")]
        [InlineData("", 200, "Saída", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Saída", "Valor deve ser maior que zero")]
        [InlineData("Teste", -200, "Saída", "Valor deve ser maior que zero")]
        [InlineData("Teste", 200, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AtualizarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            int id = 1;
            DateTime dtVencimento = DateTime.Now.AddDays(10);
            var financeiroExistente = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroExistente);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task RemoverAsync_RemoveFinanceiro_QuandoFinanceiroExiste()
        {
            // Arrange
            int id = 1;
            var financeiro = new Financeiro { Id = id };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var resultado = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.True(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_RetornaFalso_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.False(resultado);
        }
    }
}