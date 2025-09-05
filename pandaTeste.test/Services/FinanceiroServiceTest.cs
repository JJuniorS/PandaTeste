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
            // Arrange: Inicializa os mocks e o serviço a ser testado
            _financeiroRepositoryMock = new Mock<IFinanceiroRepository>();
            _financeiroService = new FinanceiroService(_financeiroRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaFinanceiro_QuandoIdExiste()
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
        public async Task ObterPorIdAsync_RetornaNull_QuandoIdNaoExiste()
        {
            // Arrange
            var id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var financeiro = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Null(financeiro);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaDeFinanceiros()
        {
            // Arrange
            var listaFinanceiros = new List<Financeiro> {
                new Financeiro { Id = 1, Descricao = "Teste 1" },
                new Financeiro { Id = 2, Descricao = "Teste 2" }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(listaFinanceiros);

            // Act
            var financeiros = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(financeiros);
            Assert.Equal(2, financeiros.Count());
        }

        [Fact]
        public async Task ObterPorTipoAsync_RetornaListaDeFinanceiros_QuandoTipoValido()
        {
            // Arrange
            var tipo = "Entrada";
            var listaFinanceiros = new List<Financeiro> {
                new Financeiro { Id = 1, TipoFinanceiro = tipo, Descricao = "Teste 1" }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(listaFinanceiros);

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
            var tipoInvalido = "Invalido";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterPorTipoAsync(tipoInvalido));
        }

        [Fact]
        public async Task ObterPorStatusAsync_RetornaListaDeFinanceiros_QuandoStatusExiste()
        {
            // Arrange
            var baixado = true;
            var listaFinanceiros = new List<Financeiro> {
                new Financeiro { Id = 1, Baixado = baixado, Descricao = "Teste 1" }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(baixado)).ReturnsAsync(listaFinanceiros);

            // Act
            var financeiros = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.NotNull(financeiros);
            Assert.Single(financeiros);
            Assert.Equal(baixado, financeiros.First().Baixado);
        }

        [Fact]
        public async Task ObterVencimentosAsync_RetornaListaDeFinanceiros_QuandoDataInicioMenorOuIgualDataFim()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddDays(-1);
            var dataFim = DateTime.Now;
            var listaFinanceiros = new List<Financeiro> {
                new Financeiro { Id = 1, DtVencimento = DateTime.Now, Descricao = "Teste 1" }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(listaFinanceiros);

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
            var resultado = await _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Theory]
        [InlineData(null, 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("", 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", -100, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 100, "Invalido", "Tipo financeiro inválido.")]
        public async Task AdicionarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            var dtVencimento = DateTime.Now;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_AtualizaStatus_QuandoFinanceiroExiste()
        {
            // Arrange
            var id = 1;
            var baixado = true;
            var financeiro = new Financeiro { Id = id, Baixado = !baixado };
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
        public async Task AlterarStatusBaixadoAsync_NaoAtualizaStatus_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            var baixado = true;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_AlteraData_QuandoFinanceiroExiste()
        {
            // Arrange
            var id = 1;
            var novaDataVencimento = DateTime.Now.AddDays(10);
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
        public async Task AlterarDataVencimentoAsync_NaoAlteraData_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            var novaDataVencimento = DateTime.Now.AddDays(10);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_AtualizaFinanceiro_QuandoDadosValidosEFincanceiroExiste()
        {
            // Arrange
            var id = 1;
            var descricao = "Teste Atualizado";
            var valor = 200;
            var tipoFinanceiro = "Saída";
            var dtVencimento = DateTime.Now.AddDays(5);
            var financeiroExistente = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };

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

        [Fact]
        public async Task AtualizarAsync_NaoAtualizaFinanceiro_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            var descricao = "Teste Atualizado";
            var valor = 200;
            var tipoFinanceiro = "Saída";
            var dtVencimento = DateTime.Now.AddDays(5);

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.False(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Theory]
        [InlineData(null, 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("", 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", -100, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 100, "Invalido", "Tipo financeiro inválido.")]
        public async Task AtualizarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            var id = 1;
            var dtVencimento = DateTime.Now;
            var financeiroExistente = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroExistente);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
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
            var resultado = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.True(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_NaoRemoveFinanceiro_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Never);
        }
    }
}