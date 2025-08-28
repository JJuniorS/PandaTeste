using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Models;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pandaTeste.Tests.Application
{
    public class FinanceiroServiceTests
    {
        private readonly Mock<IFinanceiroRepository> _financeiroRepositoryMock;
        private readonly IFinanceiroService _financeiroService;

        public FinanceiroServiceTests()
        {
            // Arrange
            _financeiroRepositoryMock = new Mock<IFinanceiroRepository>();
            _financeiroService = new FinanceiroService(_financeiroRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdExistente_RetornaFinanceiro()
        {
            // Arrange
            int id = 1;
            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);

            // Act
            var financeiro = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.NotNull(financeiro);
            Assert.Equal(id, financeiro.Id);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdInexistente_RetornaNulo()
        {
            // Arrange
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var financeiro = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Null(financeiro);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaDeFinanceiros()
        {
            // Arrange
            var financeirosRetornados = new List<Financeiro>
            {
                new Financeiro { Id = 1, Descricao = "Teste 1", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now },
                new Financeiro { Id = 2, Descricao = "Teste 2", Valor = 200, TipoFinanceiro = "Saída", DtVencimento = DateTime.Now }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(financeirosRetornados);

            // Act
            var financeiros = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(financeiros);
            Assert.Equal(2, financeiros.Count());
        }

        [Fact]
        public async Task ObterPorTipoAsync_ComTipoValido_RetornaListaDeFinanceiros()
        {
            // Arrange
            string tipo = "Entrada";
            var financeirosRetornados = new List<Financeiro>
            {
                new Financeiro { Id = 1, Descricao = "Teste 1", Valor = 100, TipoFinanceiro = tipo, DtVencimento = DateTime.Now }
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
        public async Task ObterPorTipoAsync_ComTipoInvalido_LancaArgumentException()
        {
            // Arrange
            string tipoInvalido = "Invalido";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterPorTipoAsync(tipoInvalido));
        }

        [Fact]
        public async Task ObterPorStatusAsync_ComStatusBaixado_RetornaListaDeFinanceiros()
        {
            // Arrange
            bool baixado = true;
            var financeirosRetornados = new List<Financeiro>
            {
                new Financeiro { Id = 1, Descricao = "Teste 1", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now, Baixado = baixado }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(baixado)).ReturnsAsync(financeirosRetornados);

            // Act
            var financeiros = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.NotNull(financeiros);
            Assert.Single(financeiros);
            Assert.True(financeiros.First().Baixado);
        }

        [Fact]
        public async Task ObterVencimentosAsync_ComDatasValidas_RetornaListaDeFinanceiros()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now.AddDays(-1);
            DateTime dataFim = DateTime.Now.AddDays(1);
            var financeirosRetornados = new List<Financeiro>
            {
                new Financeiro { Id = 1, Descricao = "Teste 1", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeirosRetornados);

            // Act
            var financeiros = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.NotNull(financeiros);
            Assert.Single(financeiros);
        }

        [Fact]
        public async Task ObterVencimentosAsync_ComDataInicioMaiorQueDataFim_LancaArgumentException()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now.AddDays(1);
            DateTime dataFim = DateTime.Now.AddDays(-1);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterVencimentosAsync(dataInicio, dataFim));
        }

        [Fact]
        public async Task AdicionarAsync_ComDadosValidos_RetornaTrue()
        {
            // Arrange
            string descricao = "Teste";
            decimal valor = 100;
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
        [InlineData("", 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 100, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AdicionarAsync_ComDadosInvalidos_LancaArgumentException(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            DateTime dtVencimento = DateTime.Now;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_ComIdExistente_RetornaTrue()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now, Baixado = !baixado };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.True(result);
            Assert.Equal(baixado, financeiroRetornado.Baixado);
            Assert.NotNull(financeiroRetornado.DtBaixa);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_ComIdInexistente_RetornaFalse()
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
        public async Task AlterarDataVencimentoAsync_ComIdExistente_RetornaTrue()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(7);
            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.True(result);
            Assert.Equal(novaDataVencimento.Date, financeiroRetornado.DtVencimento.Date); //Comparando apenas a data para evitar falhas devido a precisão de tempo
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_ComIdInexistente_RetornaFalse()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(7);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AtualizarAsync_ComDadosValidos_RetornaTrue()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste Atualizado";
            decimal valor = 200;
            string tipoFinanceiro = "Saída";
            DateTime dtVencimento = DateTime.Now.AddDays(10);
            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(result);
            Assert.Equal(descricao, financeiroRetornado.Descricao);
            Assert.Equal(valor, financeiroRetornado.Valor);
            Assert.Equal(tipoFinanceiro, financeiroRetornado.TipoFinanceiro);
            Assert.Equal(dtVencimento.Date, financeiroRetornado.DtVencimento.Date); //Comparando apenas a data para evitar falhas devido a precisão de tempo
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_ComIdInexistente_RetornaFalse()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste Atualizado";
            decimal valor = 200;
            string tipoFinanceiro = "Saída";
            DateTime dtVencimento = DateTime.Now.AddDays(10);
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
        public async Task AtualizarAsync_ComDadosInvalidos_LancaArgumentException(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            int id = 1;
            DateTime dtVencimento = DateTime.Now;
            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
        }

        [Fact]
        public async Task RemoverAsync_ComIdExistente_RetornaTrue()
        {
            // Arrange
            int id = 1;
            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);
            _financeiroRepositoryMock.Setup(repo => repo.RemoverAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_ComIdInexistente_RetornaFalse()
        {
            // Arrange
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var result = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.False(result);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Never);
        }
    }
}