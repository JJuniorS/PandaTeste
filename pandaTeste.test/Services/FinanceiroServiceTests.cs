using Moq;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Models;
using Xunit;

namespace pandaTeste.api.Tests.Application.Services
{
    public class FinanceiroServiceTests
    {
        private readonly Mock<IFinanceiroRepository> _mockRepository;
        private readonly FinanceiroService _financeiroService;

        public FinanceiroServiceTests()
        {
            _mockRepository = new Mock<IFinanceiroRepository>();
            _financeiroService = new FinanceiroService(_mockRepository.Object);
        }

        #region ObterPorIdAsync Tests

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarFinanceiro_QuandoIdExistir()
        {
            // Arrange
            var financeiro = new Financeiro { Id = 1, Descricao = "Teste" };
            _mockRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(financeiro);

            // Act
            var resultado = await _financeiroService.ObterPorIdAsync(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id);
            Assert.Equal("Teste", resultado.Descricao);
            _mockRepository.Verify(r => r.ObterPorIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarNull_QuandoIdNaoExistir()
        {
            // Arrange
            _mockRepository.Setup(r => r.ObterPorIdAsync(999)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.ObterPorIdAsync(999);

            // Assert
            Assert.Null(resultado);
            _mockRepository.Verify(r => r.ObterPorIdAsync(999), Times.Once);
        }

        #endregion

        #region ObterTodosAsync Tests

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarListaFinanceiros()
        {
            // Arrange
            var financeiros = new List<Financeiro>
            {
                new Financeiro { Id = 1, Descricao = "Teste 1" },
                new Financeiro { Id = 2, Descricao = "Teste 2" }
            };
            _mockRepository.Setup(r => r.ObterTodosAsync()).ReturnsAsync(financeiros);

            // Act
            var resultado = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            _mockRepository.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task ObterTodosAsync_DeveRetornarListaVazia_QuandoNaoHouverRegistros()
        {
            // Arrange
            _mockRepository.Setup(r => r.ObterTodosAsync()).ReturnsAsync(new List<Financeiro>());

            // Act
            var resultado = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            _mockRepository.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        #endregion

        #region ObterPorTipoAsync Tests

        [Theory]
        [InlineData("Entrada")]
        [InlineData("Saída")]
        public async Task ObterPorTipoAsync_DeveRetornarFinanceiros_QuandoTipoForValido(string tipo)
        {
            // Arrange
            var financeiros = new List<Financeiro>
            {
                new Financeiro { Id = 1, TipoFinanceiro = tipo }
            };
            _mockRepository.Setup(r => r.ObterPorTipoAsync(tipo)).ReturnsAsync(financeiros);

            // Act
            var resultado = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal(tipo, resultado.First().TipoFinanceiro);
            _mockRepository.Verify(r => r.ObterPorTipoAsync(tipo), Times.Once);
        }

        [Theory]
        [InlineData("TipoInvalido")]
        [InlineData("")]
        [InlineData("entrada")]
        [InlineData("saída")]
        public async Task ObterPorTipoAsync_DeveLancarArgumentException_QuandoTipoForInvalido(string tipoInvalido)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _financeiroService.ObterPorTipoAsync(tipoInvalido));

            Assert.Contains("Tipo financeiro inválido", exception.Message);
            Assert.Contains("Entrada", exception.Message);
            Assert.Contains("Saída", exception.Message);
            _mockRepository.Verify(r => r.ObterPorTipoAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region ObterPorStatusAsync Tests

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ObterPorStatusAsync_DeveRetornarFinanceiros_QuandoChamado(bool baixado)
        {
            // Arrange
            var financeiros = new List<Financeiro>
            {
                new Financeiro { Id = 1, Baixado = baixado }
            };
            _mockRepository.Setup(r => r.ObterPorStatusAsync(baixado)).ReturnsAsync(financeiros);

            // Act
            var resultado = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal(baixado, resultado.First().Baixado);
            _mockRepository.Verify(r => r.ObterPorStatusAsync(baixado), Times.Once);
        }

        #endregion

        #region ObterVencimentosAsync Tests

        [Fact]
        public async Task ObterVencimentosAsync_DeveRetornarFinanceiros_QuandoDatasForemValidas()
        {
            // Arrange
            var dataInicio = new DateTime(2024, 1, 1);
            var dataFim = new DateTime(2024, 12, 31);
            var financeiros = new List<Financeiro>
            {
                new Financeiro { Id = 1, DtVencimento = new DateTime(2024, 6, 15) }
            };
            _mockRepository.Setup(r => r.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeiros);

            // Act
            var resultado = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            _mockRepository.Verify(r => r.ObterVencimentosAsync(dataInicio, dataFim), Times.Once);
        }

        [Fact]
        public async Task ObterVencimentosAsync_DeveLancarArgumentException_QuandoDataInicioForMaiorQueDataFim()
        {
            // Arrange
            var dataInicio = new DateTime(2024, 12, 31);
            var dataFim = new DateTime(2024, 1, 1);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _financeiroService.ObterVencimentosAsync(dataInicio, dataFim));

            Assert.Equal("Data início não pode ser maior que data fim", exception.Message);
            _mockRepository.Verify(r => r.ObterVencimentosAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        #endregion

        #region AdicionarAsync Tests

        [Fact]
        public async Task AdicionarAsync_DeveRetornarTrue_QuandoParametrosForemValidos()
        {
            // Arrange
            var descricao = "Teste Financeiro";
            var valor = 100.50m;
            var tipo = "Entrada";
            var dataVencimento = DateTime.Now.AddDays(30);

            _mockRepository.Setup(r => r.AdicionarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AdicionarAsync(descricao, valor, tipo, dataVencimento);

            // Assert
            Assert.True(resultado);
            _mockRepository.Verify(r => r.AdicionarAsync(It.Is<Financeiro>(f =>
                f.Descricao == descricao.Trim() &&
                f.Valor == valor &&
                f.TipoFinanceiro == tipo &&
                f.DtVencimento == dataVencimento &&
                f.Baixado == false)), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AdicionarAsync_DeveLancarArgumentException_QuandoDescricaoForInvalida(string descricao)
        {
            // Arrange
            var valor = 100.50m;
            var tipo = "Entrada";
            var dataVencimento = DateTime.Now.AddDays(30);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _financeiroService.AdicionarAsync(descricao, valor, tipo, dataVencimento));

            Assert.Equal("Descrição é obrigatória", exception.Message);
            _mockRepository.Verify(r => r.AdicionarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100.50)]
        public async Task AdicionarAsync_DeveLancarArgumentException_QuandoValorForMenorOuIgualZero(decimal valor)
        {
            // Arrange
            var descricao = "Teste";
            var tipo = "Entrada";
            var dataVencimento = DateTime.Now.AddDays(30);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _financeiroService.AdicionarAsync(descricao, valor, tipo, dataVencimento));

            Assert.Equal("Valor deve ser maior que zero", exception.Message);
            _mockRepository.Verify(r => r.AdicionarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Theory]
        [InlineData("TipoInvalido")]
        [InlineData("entrada")]
        [InlineData("ENTRADA")]
        [InlineData("")]
        public async Task AdicionarAsync_DeveLancarArgumentException_QuandoTipoForInvalido(string tipo)
        {
            // Arrange
            var descricao = "Teste";
            var valor = 100.50m;
            var dataVencimento = DateTime.Now.AddDays(30);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _financeiroService.AdicionarAsync(descricao, valor, tipo, dataVencimento));

            Assert.Contains("Tipo financeiro inválido", exception.Message);
            _mockRepository.Verify(r => r.AdicionarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AdicionarAsync_DeveTrimmearDescricao_QuandoDescricaoTiverEspacos()
        {
            // Arrange
            var descricao = "  Teste Financeiro  ";
            var valor = 100.50m;
            var tipo = "Entrada";
            var dataVencimento = DateTime.Now.AddDays(30);

            _mockRepository.Setup(r => r.AdicionarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            await _financeiroService.AdicionarAsync(descricao, valor, tipo, dataVencimento);

            // Assert
            _mockRepository.Verify(r => r.AdicionarAsync(It.Is<Financeiro>(f =>
                f.Descricao == "Teste Financeiro")), Times.Once);
        }

        #endregion

        #region AlterarStatusBaixadoAsync Tests

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AlterarStatusBaixadoAsync_DeveRetornarTrue_QuandoFinanceiroExistir(bool novoBaixado)
        {
            // Arrange
            var financeiro = new Financeiro { Id = 1, Baixado = !novoBaixado };
            _mockRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(financeiro);
            _mockRepository.Setup(r => r.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(1, novoBaixado);

            // Assert
            Assert.True(resultado);
            Assert.Equal(novoBaixado, financeiro.Baixado);
            if (novoBaixado)
            {
                Assert.NotNull(financeiro.DtBaixa);
            }
            else
            {
                Assert.Null(financeiro.DtBaixa);
            }
            _mockRepository.Verify(r => r.ObterPorIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.AtualizarAsync(financeiro), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_DeveRetornarFalse_QuandoFinanceiroNaoExistir()
        {
            // Arrange
            _mockRepository.Setup(r => r.ObterPorIdAsync(999)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(999, true);

            // Assert
            Assert.False(resultado);
            _mockRepository.Verify(r => r.ObterPorIdAsync(999), Times.Once);
            _mockRepository.Verify(r => r.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        #endregion

        #region AlterarDataVencimentoAsync Tests

        [Fact]
        public async Task AlterarDataVencimentoAsync_DeveRetornarTrue_QuandoFinanceiroExistir()
        {
            // Arrange
            var financeiro = new Financeiro { Id = 1, DtVencimento = DateTime.Now };
            var novaData = DateTime.Now.AddDays(30);
            _mockRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(financeiro);
            _mockRepository.Setup(r => r.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(1, novaData);

            // Assert
            Assert.True(resultado);
            Assert.Equal(novaData, financeiro.DtVencimento);
            _mockRepository.Verify(r => r.ObterPorIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.AtualizarAsync(financeiro), Times.Once);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_DeveRetornarFalse_QuandoFinanceiroNaoExistir()
        {
            // Arrange
            var novaData = DateTime.Now.AddDays(30);
            _mockRepository.Setup(r => r.ObterPorIdAsync(999)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(999, novaData);

            // Assert
            Assert.False(resultado);
            _mockRepository.Verify(r => r.ObterPorIdAsync(999), Times.Once);
            _mockRepository.Verify(r => r.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        #endregion

        #region AtualizarAsync Tests

        [Fact]
        public async Task AtualizarAsync_DeveRetornarTrue_QuandoParametrosForemValidosEFinanceiroExistir()
        {
            // Arrange
            var financeiro = new Financeiro { Id = 1 };
            var descricao = "Nova Descrição";
            var valor = 200.75m;
            var tipo = "Saída";
            var dataVencimento = DateTime.Now.AddDays(45);

            _mockRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(financeiro);
            _mockRepository.Setup(r => r.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(1, descricao, valor, tipo, dataVencimento);

            // Assert
            Assert.True(resultado);
            Assert.Equal(descricao, financeiro.Descricao);
            Assert.Equal(valor, financeiro.Valor);
            Assert.Equal(tipo, financeiro.TipoFinanceiro);
            Assert.Equal(dataVencimento, financeiro.DtVencimento);
            _mockRepository.Verify(r => r.ObterPorIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.AtualizarAsync(financeiro), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_DeveRetornarFalse_QuandoFinanceiroNaoExistir()
        {
            // Arrange
            _mockRepository.Setup(r => r.ObterPorIdAsync(999)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(999, "Teste", 100m, "Entrada", DateTime.Now);

            // Assert
            Assert.False(resultado);
            _mockRepository.Verify(r => r.ObterPorIdAsync(999), Times.Once);
            _mockRepository.Verify(r => r.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task AtualizarAsync_DeveLancarArgumentException_QuandoDescricaoForInvalida(string descricao)
        {
            // Arrange
            var financeiro = new Financeiro { Id = 1 };
            _mockRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(financeiro);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _financeiroService.AtualizarAsync(1, descricao, 100m, "Entrada", DateTime.Now));

            Assert.Equal("Descrição é obrigatória", exception.Message);
            _mockRepository.Verify(r => r.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100.50)]
        public async Task AtualizarAsync_DeveLancarArgumentException_QuandoValorForMenorOuIgualZero(decimal valor)
        {
            // Arrange
            var financeiro = new Financeiro { Id = 1 };
            _mockRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(financeiro);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _financeiroService.AtualizarAsync(1, "Teste", valor, "Entrada", DateTime.Now));

            Assert.Equal("Valor deve ser maior que zero", exception.Message);
            _mockRepository.Verify(r => r.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Theory]
        [InlineData("TipoInvalido")]
        [InlineData("")]
        public async Task AtualizarAsync_DeveLancarArgumentException_QuandoTipoForInvalido(string tipo)
        {
            // Arrange
            var financeiro = new Financeiro { Id = 1 };
            _mockRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(financeiro);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _financeiroService.AtualizarAsync(1, "Teste", 100m, tipo, DateTime.Now));

            Assert.Contains("Tipo financeiro inválido", exception.Message);
            _mockRepository.Verify(r => r.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        #endregion

        #region RemoverAsync Tests

        [Fact]
        public async Task RemoverAsync_DeveRetornarTrue_QuandoFinanceiroExistir()
        {
            // Arrange
            var financeiro = new Financeiro { Id = 1 };
            _mockRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(financeiro);
            _mockRepository.Setup(r => r.RemoverAsync(1)).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.RemoverAsync(1);

            // Assert
            Assert.True(resultado);
            _mockRepository.Verify(r => r.ObterPorIdAsync(1), Times.Once);
            _mockRepository.Verify(r => r.RemoverAsync(1), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_DeveRetornarFalse_QuandoFinanceiroNaoExistir()
        {
            // Arrange
            _mockRepository.Setup(r => r.ObterPorIdAsync(999)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.RemoverAsync(999);

            // Assert
            Assert.False(resultado);
            _mockRepository.Verify(r => r.ObterPorIdAsync(999), Times.Once);
            _mockRepository.Verify(r => r.RemoverAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion
    }
}