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
            _financeiroRepositoryMock = new Mock<IFinanceiroRepository>();
            _financeiroService = new FinanceiroService(_financeiroRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdExistente_RetornaFinanceiro()
        {
            // Arrange
            int id = 1;
            var financeiro = new Financeiro { Id = id, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var resultado = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(id, resultado.Id);
            Assert.Equal("Teste", resultado.Descricao);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdInexistente_RetornaNull()
        {
            // Arrange
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Null(resultado);
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
            var resultado = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.Collection(resultado,
                item => Assert.Equal("Teste1", item.Descricao),
                item => Assert.Equal("Teste2", item.Descricao)
            );
        }

        [Fact]
        public async Task ObterPorTipoAsync_ComTipoValido_RetornaListaDeFinanceiros()
        {
            // Arrange
            string tipo = "Entrada";
            var financeiros = new List<Financeiro> {
                new Financeiro { Id = 1, Descricao = "Teste1", TipoFinanceiro = tipo },
                new Financeiro { Id = 2, Descricao = "Teste2", TipoFinanceiro = tipo }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(financeiros);

            // Act
            var resultado = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.All(resultado, item => Assert.Equal(tipo, item.TipoFinanceiro));
        }

        [Fact]
        public async Task ObterPorTipoAsync_ComTipoInvalido_LancaExcecao()
        {
            // Arrange
            string tipo = "Invalido";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterPorTipoAsync(tipo));
        }

        [Fact]
        public async Task ObterPorStatusAsync_ComStatusValido_RetornaListaDeFinanceiros()
        {
            // Arrange
            bool baixado = true;
            var financeiros = new List<Financeiro> {
                new Financeiro { Id = 1, Descricao = "Teste1", Baixado = baixado },
                new Financeiro { Id = 2, Descricao = "Teste2", Baixado = baixado }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(baixado)).ReturnsAsync(financeiros);

            // Act
            var resultado = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            Assert.All(resultado, item => Assert.Equal(baixado, item.Baixado));
        }

        [Fact]
        public async Task ObterVencimentosAsync_ComDatasValidas_RetornaListaDeFinanceiros()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now.AddDays(-1);
            DateTime dataFim = DateTime.Now.AddDays(1);
            var financeiros = new List<Financeiro> {
                new Financeiro { Id = 1, Descricao = "Teste1", DtVencimento = DateTime.Now },
                new Financeiro { Id = 2, Descricao = "Teste2", DtVencimento = DateTime.Now }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeiros);

            // Act
            var resultado = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task ObterVencimentosAsync_ComDataInicioMaiorQueDataFim_LancaExcecao()
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

            // Act
            var resultado = await _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Theory]
        [InlineData(null, 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("", 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("   ", 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", -100, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 100, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AdicionarAsync_ComDadosInvalidos_LancaExcecao(string descricao, decimal valor, string tipoFinanceiro, string mensagemEsperada)
        {
            // Arrange
            DateTime dtVencimento = DateTime.Now;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemEsperada, exception.Message);
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Financeiro>()), Times.Never);
        }


        [Fact]
        public async Task AlterarStatusBaixadoAsync_ComIdExistente_RetornaTrue()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            var financeiro = new Financeiro { Id = id, Descricao = "Teste", Baixado = false };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.True(resultado);
            Assert.Equal(baixado, financeiro.Baixado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiro), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_ComIdInexistente_RetornaFalse()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_ComIdExistente_RetornaTrue()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(10);
            var financeiro = new Financeiro { Id = id, Descricao = "Teste", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.True(resultado);
            Assert.Equal(novaDataVencimento, financeiro.DtVencimento);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiro), Times.Once);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_ComIdInexistente_RetornaFalse()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(10);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_ComDadosValidosEIdExistente_RetornaTrue()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste Atualizado";
            decimal valor = 200;
            string tipoFinanceiro = "Saída";
            DateTime dtVencimento = DateTime.Now.AddDays(5);
            var financeiro = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);
            Assert.Equal(descricao, financeiro.Descricao);
            Assert.Equal(valor, financeiro.Valor);
            Assert.Equal(tipoFinanceiro, financeiro.TipoFinanceiro);
            Assert.Equal(dtVencimento, financeiro.DtVencimento);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(financeiro), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_ComIdInexistente_RetornaFalse()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste Atualizado";
            decimal valor = 200;
            string tipoFinanceiro = "Saída";
            DateTime dtVencimento = DateTime.Now.AddDays(5);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }


        [Theory]
        [InlineData(null, 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("", 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("   ", 100, "Entrada", "Descrição é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", -100, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 100, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AtualizarAsync_ComDadosInvalidos_LancaExcecao(string descricao, decimal valor, string tipoFinanceiro, string mensagemEsperada)
        {
            // Arrange
             int id = 1;
            DateTime dtVencimento = DateTime.Now;
             var financeiro = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemEsperada, exception.Message);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task RemoverAsync_ComIdExistente_RetornaTrue()
        {
            // Arrange
            int id = 1;
            var financeiro = new Financeiro { Id = id, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var resultado = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.True(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_ComIdInexistente_RetornaFalse()
        {
            // Arrange
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Never);
        }
    }
}