using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PandaTeste.Tests
{
    public class FinanceiroServiceTests
    {
        private readonly Mock<IFinanceiroRepository> _financeiroRepositoryMock;
        private readonly FinanceiroService _financeiroService;

        public FinanceiroServiceTests()
        {
            // Inicializa o mock do repositório e o serviço no construtor para reutilização em cada teste.
            _financeiroRepositoryMock = new Mock<IFinanceiroRepository>();
            _financeiroService = new FinanceiroService(_financeiroRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoIdExistente_RetornaFinanceiro()
        {
            // Arrange
            var id = 1;
            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);

            // Act
            var financeiro = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.NotNull(financeiro);
            Assert.Equal(id, financeiro.Id);
            Assert.Equal("Teste", financeiro.Descricao);
        }

        [Fact]
        public async Task ObterPorIdAsync_QuandoIdNaoExistente_RetornaNulo()
        {
            // Arrange
            var id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var financeiro = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Null(financeiro);
        }

        [Fact]
        public async Task ObterTodosAsync_QuandoExistiremFinanceiros_RetornaLista()
        {
            // Arrange
            var listaFinanceiros = new List<Financeiro> { new Financeiro { Id = 1 }, new Financeiro { Id = 2 } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(listaFinanceiros);

            // Act
            var financeiros = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(financeiros);
            Assert.Equal(2, financeiros.Count());
        }

        [Fact]
        public async Task ObterTodosAsync_QuandoNaoExistiremFinanceiros_RetornaListaVazia()
        {
            // Arrange
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(new List<Financeiro>());

            // Act
            var financeiros = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(financeiros);
            Assert.Empty(financeiros);
        }

        [Theory]
        [InlineData("Entrada")]
        [InlineData("Saída")]
        public async Task ObterPorTipoAsync_QuandoTipoValido_RetornaListaDeFinanceiros(string tipo)
        {
            // Arrange
            var listaFinanceiros = new List<Financeiro> { new Financeiro { Id = 1, TipoFinanceiro = tipo } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(listaFinanceiros);

            // Act
            var financeiros = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.NotNull(financeiros);
            Assert.Single(financeiros);
            Assert.Equal(tipo, financeiros.First().TipoFinanceiro);
        }

        [Theory]
        [InlineData("Invalido")]
        [InlineData("")]
        [InlineData(null)]
        public async Task ObterPorTipoAsync_QuandoTipoInvalido_LancaExcecao(string tipoInvalido)
        {
            // Arrange
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterPorTipoAsync(tipoInvalido));
        }

        [Fact]
        public async Task ObterPorStatusAsync_QuandoExistiremFinanceirosComStatus_RetornaLista()
        {
            // Arrange
            var status = true;
            var listaFinanceiros = new List<Financeiro> { new Financeiro { Id = 1, Baixado = status } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(status)).ReturnsAsync(listaFinanceiros);

            // Act
            var financeiros = await _financeiroService.ObterPorStatusAsync(status);

            // Assert
            Assert.NotNull(financeiros);
            Assert.Single(financeiros);
            Assert.Equal(status, financeiros.First().Baixado);
        }

        [Fact]
        public async Task ObterVencimentosAsync_QuandoDataInicioMaiorQueDataFim_LancaExcecao()
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
            var dataFim = DateTime.Now.AddDays(7);
            var listaFinanceiros = new List<Financeiro> { new Financeiro { Id = 1, DtVencimento = dataInicio } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(listaFinanceiros);

            // Act
            var financeiros = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.NotNull(financeiros);
            Assert.Single(financeiros);
            Assert.Equal(dataInicio, financeiros.First().DtVencimento);
        }

        [Fact]
        public async Task AdicionarAsync_QuandoDescricaoInvalida_LancaExcecao()
        {
            // Arrange
            string descricao = null;
            decimal valor = 10;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
        }

        [Fact]
        public async Task AdicionarAsync_QuandoValorInvalido_LancaExcecao()
        {
            // Arrange
            string descricao = "Teste";
            decimal valor = 0;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
        }

        [Fact]
        public async Task AdicionarAsync_QuandoTipoFinanceiroInvalido_LancaExcecao()
        {
            // Arrange
            string descricao = "Teste";
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
            string descricao = "Teste";
            decimal valor = 10;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;

            _financeiroRepositoryMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.Is<Financeiro>(f => f.Descricao == descricao && f.Valor == valor && f.TipoFinanceiro == tipoFinanceiro)), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_QuandoFinanceiroNaoExiste_RetornaFalse()
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
        public async Task AlterarStatusBaixadoAsync_QuandoFinanceiroExiste_AtualizaStatusERetornaTrue()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            var financeiro = new Financeiro { Id = id, Baixado = false };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.True(result);
            Assert.Equal(baixado, financeiro.Baixado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.Is<Financeiro>(f => f.Id == id && f.Baixado == baixado)), Times.Once);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_QuandoFinanceiroNaoExiste_RetornaFalse()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(10);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_QuandoFinanceiroExiste_AtualizaDataVencimentoERetornaTrue()
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
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.Is<Financeiro>(f => f.Id == id && f.DtVencimento == novaDataVencimento)), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_QuandoFinanceiroNaoExiste_RetornaFalse()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste";
            decimal valor = 10;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AtualizarAsync_QuandoDescricaoInvalida_LancaExcecao()
        {
            // Arrange
            int id = 1;
            string descricao = null;
            decimal valor = 10;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;
            var financeiro = new Financeiro { Id = id };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
        }

        [Fact]
        public async Task AtualizarAsync_QuandoValorInvalido_LancaExcecao()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste";
            decimal valor = 0;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now;
            var financeiro = new Financeiro { Id = id };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
        }

        [Fact]
        public async Task AtualizarAsync_QuandoTipoFinanceiroInvalido_LancaExcecao()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste";
            decimal valor = 10;
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
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.Is<Financeiro>(f => f.Id == id && f.Descricao == descricao && f.Valor == valor && f.TipoFinanceiro == tipoFinanceiro && f.DtVencimento == dtVencimento)), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_QuandoFinanceiroNaoExiste_RetornaFalse()
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
        public async Task RemoverAsync_QuandoFinanceiroExiste_RemoveFinanceiroERetornaTrue()
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