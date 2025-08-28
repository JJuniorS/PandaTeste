using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Models;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pandaTeste.Tests.Services
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
            int id = 1;
            var financeiroRetornado = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroRetornado);

            // Act
            var financeiro = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.NotNull(financeiro);
            Assert.Equal(id, financeiro.Id);
            _financeiroRepositoryMock.Verify(repo => repo.ObterPorIdAsync(id), Times.Once); //Verifica se o método foi chamado apenas uma vez.
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaNull_QuandoIdNaoExiste()
        {
            // Arrange
            int id = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro); //Garante que retorna null

            // Act
            var financeiro = await _financeiroService.ObterPorIdAsync(id);

            // Assert
            Assert.Null(financeiro);
            _financeiroRepositoryMock.Verify(repo => repo.ObterPorIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaDeFinanceiros()
        {
            // Arrange
            var financeiroList = new List<Financeiro> {
                new Financeiro { Id = 1, Descricao = "Teste1", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now },
                new Financeiro { Id = 2, Descricao = "Teste2", Valor = 200, TipoFinanceiro = "Saída", DtVencimento = DateTime.Now }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(financeiroList);

            // Act
            var financeiros = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(financeiros);
            Assert.IsAssignableFrom<IEnumerable<Financeiro>>(financeiros);
            Assert.Equal(2, ((List<Financeiro>)financeiros).Count);
            _financeiroRepositoryMock.Verify(repo => repo.ObterTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaVazia_QuandoNaoHaFinanceiros()
        {
            // Arrange
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(new List<Financeiro>());

            // Act
            var financeiros = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(financeiros);
            Assert.Empty(financeiros);
            _financeiroRepositoryMock.Verify(repo => repo.ObterTodosAsync(), Times.Once);
        }

        [Theory]
        [InlineData("Entrada")]
        [InlineData("Saída")]
        public async Task ObterPorTipoAsync_RetornaFinanceirosDoTipo_QuandoTipoValido(string tipo)
        {
            // Arrange
            var financeiroList = new List<Financeiro> {
                new Financeiro { Id = 1, Descricao = "Teste1", Valor = 100, TipoFinanceiro = tipo, DtVencimento = DateTime.Now },
                new Financeiro { Id = 2, Descricao = "Teste2", Valor = 200, TipoFinanceiro = tipo, DtVencimento = DateTime.Now }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(financeiroList);

            // Act
            var financeiros = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.NotNull(financeiros);
            Assert.All(financeiros, f => Assert.Equal(tipo, f.TipoFinanceiro));
            _financeiroRepositoryMock.Verify(repo => repo.ObterPorTipoAsync(tipo), Times.Once);
        }

        [Theory]
        [InlineData("Invalido")]
        [InlineData("")]
        [InlineData(null)]
        public async Task ObterPorTipoAsync_LancaExcecao_QuandoTipoInvalido(string tipoInvalido)
        {
            // Arrange
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterPorTipoAsync(tipoInvalido));
            _financeiroRepositoryMock.Verify(repo => repo.ObterPorTipoAsync(It.IsAny<string>()), Times.Never); //Verifica que nunca chamou o repository
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ObterPorStatusAsync_RetornaFinanceirosComStatusCorreto(bool status)
        {
            // Arrange
            var financeiroList = new List<Financeiro> {
                new Financeiro { Id = 1, Descricao = "Teste1", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now, Baixado = status },
                new Financeiro { Id = 2, Descricao = "Teste2", Valor = 200, TipoFinanceiro = "Saída", DtVencimento = DateTime.Now, Baixado = status }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(status)).ReturnsAsync(financeiroList);

            // Act
            var financeiros = await _financeiroService.ObterPorStatusAsync(status);

            // Assert
            Assert.NotNull(financeiros);
            Assert.All(financeiros, f => Assert.Equal(status, f.Baixado));
            _financeiroRepositoryMock.Verify(repo => repo.ObterPorStatusAsync(status), Times.Once);
        }

        [Fact]
        public async Task ObterVencimentosAsync_RetornaFinanceirosNoPeriodo_QuandoDatasValidas()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now.AddDays(-10);
            DateTime dataFim = DateTime.Now.AddDays(10);
            var financeiroList = new List<Financeiro> {
                new Financeiro { Id = 1, Descricao = "Teste1", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now, Baixado = false },
                new Financeiro { Id = 2, Descricao = "Teste2", Valor = 200, TipoFinanceiro = "Saída", DtVencimento = DateTime.Now, Baixado = true }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeiroList);

            // Act
            var financeiros = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.NotNull(financeiros);
            _financeiroRepositoryMock.Verify(repo => repo.ObterVencimentosAsync(dataInicio, dataFim), Times.Once);
        }

        [Fact]
        public async Task ObterVencimentosAsync_LancaExcecao_QuandoDataInicioMaiorQueDataFim()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now.AddDays(10);
            DateTime dataFim = DateTime.Now.AddDays(-10);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterVencimentosAsync(dataInicio, dataFim));
            _financeiroRepositoryMock.Verify(repo => repo.ObterVencimentosAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
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
        [InlineData("", 100, "Entrada", "Descricao é obrigatória")]
        [InlineData(" ", 100, "Entrada", "Descricao é obrigatória")]
        [InlineData(null, 100, "Entrada", "Descricao é obrigatória")]
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
            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_AtualizaStatus_QuandoFinanceiroExiste()
        {
            // Arrange
            int id = 1;
            bool baixado = true;
            var financeiro = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now, Baixado = !baixado };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var result = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.Is<Financeiro>(f => f.Id == id && f.Baixado == baixado)), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_NaoAtualiza_QuandoFinanceiroNaoExiste()
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
        public async Task AlterarDataVencimentoAsync_AtualizaData_QuandoFinanceiroExiste()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(30);
            var financeiro = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var result = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.Is<Financeiro>(f => f.Id == id && f.DtVencimento == novaDataVencimento)), Times.Once);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_NaoAtualiza_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            int id = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(30);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

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
            int id = 1;
            string descricao = "Teste Atualizado";
            decimal valor = 200;
            string tipoFinanceiro = "Saída";
            DateTime dtVencimento = DateTime.Now.AddDays(15);
            var financeiro = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.Is<Financeiro>(f =>
                f.Id == id &&
                f.Descricao == descricao &&
                f.Valor == valor &&
                f.TipoFinanceiro == tipoFinanceiro &&
                f.DtVencimento == dtVencimento
            )), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_NaoAtualiza_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            int id = 1;
            string descricao = "Teste Atualizado";
            decimal valor = 200;
            string tipoFinanceiro = "Saída";
            DateTime dtVencimento = DateTime.Now.AddDays(15);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var result = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.False(result);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Theory]
        [InlineData("", 100, "Entrada", "Descricao é obrigatória")]
        [InlineData(" ", 100, "Entrada", "Descricao é obrigatória")]
        [InlineData(null, 100, "Entrada", "Descricao é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", -100, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 100, "Invalido", "Tipo financeiro inválido. Use: Entrada, Saída")]
        public async Task AtualizarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            int id = 1;
            DateTime dtVencimento = DateTime.Now;
            var financeiro = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, exception.Message);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task RemoverAsync_RemoveFinanceiro_QuandoFinanceiroExiste()
        {
            // Arrange
            int id = 1;
            var financeiro = new Financeiro { Id = id, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiro);

            // Act
            var result = await _financeiroService.RemoverAsync(id);

            // Assert
            Assert.True(result);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_NaoRemove_QuandoFinanceiroNaoExiste()
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