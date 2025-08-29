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
            // Arrange: Inicializa os mocks e o serviço antes de cada teste.
            _financeiroRepositoryMock = new Mock<IFinanceiroRepository>();
            _financeiroService = new FinanceiroService(_financeiroRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoIdExistente_RetornaFinanceiro()
        {
            // Arrange
            var id = 1;
            var financeiroEsperado = new Financeiro { Id = id, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroEsperado);

            // Act
            var financeiroRetornado = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Equal(financeiroEsperado, financeiroRetornado);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoIdNaoExistente_RetornaNull()
        {
            // Arrange
            var id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var financeiroRetornado = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Null(financeiroRetornado);
        }

        [Fact]
        public async Task ObterTodosAsync_QuandoExistemFinanceiros_RetornaLista()
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
        public async Task ObterTodosAsync_QuandoNaoExistemFinanceiros_RetornaListaVazia()
        {
            // Arrange
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(new List<Financeiro>());

            // Act
            var financeirosRetornados = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.Empty(financeirosRetornados);
        }

        [Theory]
        [InlineData("Entrada")]
        [InlineData("Saída")]
        public async Task ObterPorTipoAsync_QuandoTipoValido_RetornaListaDeFinanceiros(string tipo)
        {
            // Arrange
            var financeirosEsperados = new List<Financeiro> { new Financeiro { TipoFinanceiro = tipo } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(financeirosEsperados);

            // Act
            var financeirosRetornados = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.Equal(financeirosEsperados, financeirosRetornados);
        }

        [Theory]
        [InlineData("Invalido")]
        [InlineData("Outro")]
        public async Task ObterPorTipoAsync_QuandoTipoInvalido_LancaArgumentException(string tipo)
        {
            // Arrange
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterPorTipoAsync(tipo));
        }

        [Fact]
        public async Task ObterPorStatusAsync_QuandoExistemFinanceirosComStatus_RetornaLista()
        {
            // Arrange
            var baixado = true;
            var financeirosEsperados = new List<Financeiro> { new Financeiro { Baixado = baixado } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(baixado)).ReturnsAsync(financeirosEsperados);

            // Act
            var financeirosRetornados = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.Equal(financeirosEsperados, financeirosRetornados);
        }

        [Fact]
        public async Task ObterVencimentosAsync_QuandoDataInicioMaiorQueFim_LancaArgumentException()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddDays(1);
            var dataFim = DateTime.Now;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterVencimentosAsync(dataInicio, dataFim));
        }

        [Fact]
        public async Task ObterVencimentosAsync_QuandoDatasValidas_RetornaListaDeFinanceiros()
        {
            // Arrange
            var dataInicio = DateTime.Now;
            var dataFim = DateTime.Now.AddDays(1);
            var financeirosEsperados = new List<Financeiro> { new Financeiro { DtVencimento = dataInicio } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeirosEsperados);

            // Act
            var financeirosRetornados = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.Equal(financeirosEsperados, financeirosRetornados);
        }

        [Fact]
        public async Task AdicionarAsync_QuandoDescricaoVazia_LancaArgumentException()
        {
            // Arrange
            string descricao = "";
            decimal valor = 10;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
        }

        [Fact]
        public async Task AdicionarAsync_QuandoValorZero_LancaArgumentException()
        {
            // Arrange
            string descricao = "Descricao";
            decimal valor = 0;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
        }

        [Fact]
        public async Task AdicionarAsync_QuandoTipoInvalido_LancaArgumentException()
        {
             // Arrange
            string descricao = "Descricao";
            decimal valor = 10;
            string tipoFinanceiro = "Invalido";
            DateTime dtVencimento = DateTime.Now;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
        }

        [Fact]
        public async Task AdicionarAsync_QuandoDadosValidos_AdicionaFinanceiroERetornaTrue()
        {
             // Arrange
            string descricao = "Descricao";
            decimal valor = 10;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

             _financeiroRepositoryMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_QuandoFinanceiroNaoEncontrado_RetornaFalse()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_QuandoFinanceiroEncontrado_AtualizaStatusERetornaTrue()
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
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiro), Times.Once);
        }

          [Fact]
        public async Task AlterarDataVencimentoAsync_QuandoFinanceiroNaoEncontrado_RetornaFalse()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(30);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_QuandoFinanceiroEncontrado_AtualizaDataVencimentoERetornaTrue()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(30);
            var financeiro = new Financeiro { Id = id, DtVencimento = DateTime.Now };

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.True(result);
            Assert.Equal(novaDataVencimento, financeiro.DtVencimento);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiro), Times.Once);
        }

         [Fact]
        public async Task AtualizarAsync_QuandoFinanceiroNaoEncontrado_RetornaFalse()
        {
            // Arrange
            int id = 1;
            string descricao = "Descrição";
            decimal valor = 100;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AtualizarAsync_QuandoDescricaoVazia_LancaArgumentException()
        {
            // Arrange
            int id = 1;
            string descricao = "";
            decimal valor = 100;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            var financeiro = new Financeiro { Id = id };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
        }

        [Fact]
        public async Task AtualizarAsync_QuandoValorZero_LancaArgumentException()
        {
            // Arrange
            int id = 1;
            string descricao = "Descrição";
            decimal valor = 0;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            var financeiro = new Financeiro { Id = id };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
        }

        [Fact]
        public async Task AtualizarAsync_QuandoTipoInvalido_LancaArgumentException()
        {
            // Arrange
            int id = 1;
            string descricao = "Descrição";
            decimal valor = 100;
            string tipoFinanceiro = "Invalido";
            DateTime dtVencimento = DateTime.Now;

            var financeiro = new Financeiro { Id = id };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
        }

        [Fact]
        public async Task AtualizarAsync_QuandoDadosValidos_AtualizaFinanceiroERetornaTrue()
        {
            // Arrange
            int id = 1;
            string descricao = "Descrição";
            decimal valor = 100;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            var financeiro = new Financeiro { Id = id, Descricao = "Old", Valor = 50, TipoFinanceiro = "Saída", DtVencimento = DateTime.Now.AddDays(-1) };

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
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiro), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_QuandoFinanceiroNaoEncontrado_RetornaFalse()
        {
            // Arrange
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RemoverAsync_QuandoFinanceiroEncontrado_RemoveFinanceiroERetornaTrue()
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

    }
}