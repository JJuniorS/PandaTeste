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
        private readonly IFinanceiroService _financeiroService;

        public FinanceiroServiceTests()
        {
            // Arrange (no construtor, para ser reutilizado em todos os testes)
            _financeiroRepositoryMock = new Mock<IFinanceiroRepository>();
            _financeiroService = new FinanceiroService(_financeiroRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaFinanceiro_QuandoIdExiste()
        {
            // Arrange
            int id = 1;
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
        public async Task ObterPorIdAsync_RetornaNull_QuandoIdNaoExiste()
        {
            // Arrange
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var financeiro = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Null(financeiro);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaFinanceiros()
        {
            // Arrange
            var financeirosRetornados = new List<Financeiro> {
                new Financeiro { Id = 1, Descricao = "Teste 1" },
                new Financeiro { Id = 2, Descricao = "Teste 2" }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(financeirosRetornados);

            // Act
            var financeiros = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(financeiros);
            Assert.Equal(2, financeiros.Count());
        }

        [Fact]
        public async Task ObterPorTipoAsync_RetornaListaFinanceiros_QuandoTipoValido()
        {
            // Arrange
            string tipo = "Entrada";
            var financeirosRetornados = new List<Financeiro> {
                new Financeiro { Id = 1, TipoFinanceiro = tipo, Descricao = "Teste 1" }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(financeirosRetornados);

            // Act
            var financeiros = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.NotNull(financeiros);
            Assert.Single(financeiros);
            Assert.Equal(tipo, financeiros.First().TipoFinanceiro);
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
        public async Task ObterPorStatusAsync_RetornaListaFinanceiros_QuandoStatusExiste()
        {
            // Arrange
            bool baixado = true;
            var financeirosRetornados = new List<Financeiro> {
                new Financeiro { Id = 1, Baixado = baixado, Descricao = "Teste 1" }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(baixado)).ReturnsAsync(financeirosRetornados);

            // Act
            var financeiros = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.NotNull(financeiros);
            Assert.Single(financeiros);
            Assert.Equal(baixado, financeiros.First().Baixado);
        }

        [Fact]
        public async Task ObterVencimentosAsync_RetornaListaFinanceiros_QuandoDataInicioMenorOuIgualDataFim()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now.AddDays(-1);
            DateTime dataFim = DateTime.Now;

            var financeirosRetornados = new List<Financeiro>
            {
                new Financeiro { Id = 1, DtVencimento = DateTime.Now, Descricao = "Teste 1" }
            };

            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeirosRetornados);

            // Act
            var financeiros = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.NotNull(financeiros);
            Assert.Single(financeiros);
        }

        [Fact]
        public async Task ObterVencimentosAsync_LancaExcecao_QuandoDataInicioMaiorDataFim()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now;
            DateTime dataFim = DateTime.Now.AddDays(-1);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterVencimentosAsync(dataInicio, dataFim));
        }

        [Fact]
        public async Task AdicionarAsync_AdicionaNovoFinanceiro_QuandoDadosValidos()
        {
            // Arrange
            string descricao = "Nova Despesa";
            decimal valor = 100.00m;
            string tipoFinanceiro = "Saída";
            DateTime dtVencimento = DateTime.Now.AddDays(30);

            // Act
            var resultado = await _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.Is<Financeiro>(f =>
                f.Descricao == descricao &&
                f.Valor == valor &&
                f.TipoFinanceiro == tipoFinanceiro &&
                f.DtVencimento == dtVencimento
            )), Times.Once);
        }

        [Theory]
        [InlineData("", 100, "Saída", "Descrição é obrigatória")]
        [InlineData("Descricao", 0, "Saída", "Valor deve ser maior que zero")]
        [InlineData("Descricao", 100, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AdicionarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            DateTime dtVencimento = DateTime.Now.AddDays(30);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_AtualizaStatusFinanceiro_QuandoIdExiste()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.True(resultado);
            Assert.Equal(baixado, financeiroRetornado.Baixado);

            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.Is<Financeiro>(f =>
                f.Id == id && f.Baixado == baixado
            )), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_NaoFazNada_QuandoIdNaoExiste()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_AtualizaDataVencimento_QuandoIdExiste()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(60);
            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.True(resultado);
            Assert.Equal(novaDataVencimento, financeiroRetornado.DtVencimento);

            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.Is<Financeiro>(f =>
                f.Id == id && f.DtVencimento == novaDataVencimento
            )), Times.Once);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_NaoFazNada_QuandoIdNaoExiste()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(60);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_AtualizaFinanceiro_QuandoDadosValidosEIdExiste()
        {
            // Arrange
            int id = 1;
            string descricao = "Descricao Atualizada";
            decimal valor = 200.00m;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now.AddDays(90);

            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Saida", DtVencimento = DateTime.Now };

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);

            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.Is<Financeiro>(f =>
                f.Id == id &&
                f.Descricao == descricao &&
                f.Valor == valor &&
                f.TipoFinanceiro == tipoFinanceiro &&
                f.DtVencimento == dtVencimento
            )), Times.Once);
        }


        [Fact]
        public async Task AtualizarAsync_NaoFazNada_QuandoIdNaoExiste()
        {
            // Arrange
            int id = 1;
            string descricao = "Descricao Atualizada";
            decimal valor = 200.00m;
            string tipoFinanceiro = "Entrada";
            DateTime dtVencimento = DateTime.Now.AddDays(90);

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Theory]
        [InlineData("", 100, "Saída", "Descrição é obrigatória")]
        [InlineData("Descricao", 0, "Saída", "Valor deve ser maior que zero")]
        [InlineData("Descricao", 100, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AtualizarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            int id = 1;
            DateTime dtVencimento = DateTime.Now.AddDays(30);

            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Saida", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task RemoverAsync_RemoveFinanceiro_QuandoIdExiste()
        {
            // Arrange
            int id = 1;
            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);

            // Act
            var resultado = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.True(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_NaoFazNada_QuandoIdNaoExiste()
        {
            // Arrange
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Never);
        }
    }
}