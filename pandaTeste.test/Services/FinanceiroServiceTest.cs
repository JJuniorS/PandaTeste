using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Models;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PandaTeste.Tests
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
            var resultado = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Equal(financeiro, resultado);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaNull_QuandoIdNaoExiste()
        {
            // Arrange
            var id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaDeFinanceiros()
        {
            // Arrange
            var financeiros = new List<Financeiro> { new Financeiro { Descricao = "Teste1" }, new Financeiro { Descricao = "Teste2" } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(financeiros);

            // Act
            var resultado = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.Equal(financeiros, resultado);
        }

        [Fact]
        public async Task ObterPorTipoAsync_RetornaListaDeFinanceiros_QuandoTipoValido()
        {
            // Arrange
            var tipo = "Entrada";
            var financeiros = new List<Financeiro> { new Financeiro { TipoFinanceiro = tipo, Descricao = "Teste1" } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(financeiros);

            // Act
            var resultado = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.Equal(financeiros, resultado);
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
            var financeiros = new List<Financeiro> { new Financeiro { Baixado = baixado, Descricao = "Teste1" } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(baixado)).ReturnsAsync(financeiros);

            // Act
            var resultado = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.Equal(financeiros, resultado);
        }

        [Fact]
        public async Task ObterVencimentosAsync_RetornaListaDeFinanceiros_QuandoDataInicioMenorOuIgualDataFim()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddDays(-1);
            var dataFim = DateTime.Now;
            var financeiros = new List<Financeiro> { new Financeiro { DtVencimento = dataFim, Descricao = "Teste1" } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeiros);

            // Act
            var resultado = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.Equal(financeiros, resultado);
        }

        [Fact]
        public async Task ObterVencimentosAsync_LancaExcecao_QuandoDataInicioMaiorQueDataFim()
        {
            // Arrange
            var dataInicio = DateTime.Now;
            var dataFim = DateTime.Now.AddDays(-1);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterVencimentosAsync(dataInicio, dataFim));
        }

        [Fact]
        public async Task AdicionarAsync_RetornaTrue_QuandoAdicaoBemSucedida()
        {
            // Arrange
            var descricao = "Teste";
            var valor = 10;
            var tipoFinanceiro = "Entrada";
            var dtVencimento = DateTime.Now;
            _financeiroRepositoryMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);
        }

        [Theory]
        [InlineData("", 10, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 10, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AdicionarAsync_LancaExcecao_QuandoParametrosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            var dtVencimento = DateTime.Now;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_RetornaTrue_QuandoFinanceiroExiste()
        {
            // Arrange
            var id = 1;
            var baixado = true;
            var financeiro = new Financeiro { Id = id, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_RetornaFalse_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            var baixado = true;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_RetornaTrue_QuandoFinanceiroExiste()
        {
            // Arrange
            var id = 1;
            var novaDataVencimento = DateTime.Now.AddDays(10);
            var financeiro = new Financeiro { Id = id, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_RetornaFalse_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            var novaDataVencimento = DateTime.Now.AddDays(10);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task AtualizarAsync_RetornaTrue_QuandoAtualizacaoBemSucedida()
        {
            // Arrange
            var id = 1;
            var descricao = "Teste Atualizado";
            var valor = 20;
            var tipoFinanceiro = "Saída";
            var dtVencimento = DateTime.Now.AddDays(5);

            var financeiroExistente = new Financeiro { Id = id, Descricao = "Teste Antigo", Valor = 10, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroExistente);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task AtualizarAsync_RetornaFalse_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            var descricao = "Teste Atualizado";
            var valor = 20;
            var tipoFinanceiro = "Saída";
            var dtVencimento = DateTime.Now.AddDays(5);

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.False(resultado);
        }

        [Theory]
        [InlineData("", 10, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 10, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AtualizarAsync_LancaExcecao_QuandoParametrosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            var id = 1;
            var dtVencimento = DateTime.Now;

             var financeiroExistente = new Financeiro { Id = id};
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroExistente);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task RemoverAsync_RetornaTrue_QuandoRemocaoBemSucedida()
        {
            // Arrange
            var id = 1;
            var financeiro = new Financeiro { Id = id, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);
            _financeiroRepositoryMock.Setup(repo => repo.RemoverAsync(id)).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task RemoverAsync_RetornaFalse_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.False(resultado);
        }
    }
}