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
        public async Task ObterPorIdAsync_FinanceiroExiste_RetornaFinanceiro()
        {
            // Arrange
            int financeiroId = 1;
            var financeiroEsperado = new Financeiro { Id = financeiroId, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync(financeiroEsperado);

            // Act
            var financeiroObtido = await _financeiroService.ObterPorIdAsync(financeiroId);

            // Assert
            Assert.Equal(financeiroEsperado, financeiroObtido);
        }

        [Fact]
        public async Task ObterPorIdAsync_FinanceiroNaoExiste_RetornaNull()
        {
            // Arrange
            int financeiroId = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync((Financeiro)null);

            // Act
            var financeiroObtido = await _financeiroService.ObterPorIdAsync(financeiroId);

            // Assert
            Assert.Null(financeiroObtido);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaDeFinanceiros()
        {
            // Arrange
            var financeirosEsperados = new List<Financeiro>
            {
                new Financeiro { Id = 1, Descricao = "Teste1", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now },
                new Financeiro { Id = 2, Descricao = "Teste2", Valor = 200, TipoFinanceiro = "Saída", DtVencimento = DateTime.Now }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(financeirosEsperados);

            // Act
            var financeirosObtidos = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.Equal(financeirosEsperados, financeirosObtidos);
        }

        [Fact]
        public async Task ObterPorTipoAsync_TipoValido_RetornaListaDeFinanceirosDoTipo()
        {
            // Arrange
            string tipoFinanceiro = "Entrada";
            var financeirosEsperados = new List<Financeiro>
            {
                new Financeiro { Id = 1, Descricao = "Teste1", Valor = 100, TipoFinanceiro = tipoFinanceiro, DtVencimento = DateTime.Now }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipoFinanceiro)).ReturnsAsync(financeirosEsperados);

            // Act
            var financeirosObtidos = await _financeiroService.ObterPorTipoAsync(tipoFinanceiro);

            // Assert
            Assert.Equal(financeirosEsperados, financeirosObtidos);
        }

        [Fact]
        public async Task ObterPorTipoAsync_TipoInvalido_LancaArgumentException()
        {
            // Arrange
            string tipoFinanceiro = "Invalido";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterPorTipoAsync(tipoFinanceiro));
        }

        [Fact]
        public async Task ObterPorStatusAsync_RetornaListaDeFinanceirosComStatusCorreto()
        {
            // Arrange
            bool baixado = true;
            var financeirosEsperados = new List<Financeiro>
            {
                new Financeiro { Id = 1, Descricao = "Teste1", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now, Baixado = baixado }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(baixado)).ReturnsAsync(financeirosEsperados);

            // Act
            var financeirosObtidos = await _financeiroService.ObterPorStatusAsync(baixado);

            // Assert
            Assert.Equal(financeirosEsperados, financeirosObtidos);
        }

        [Fact]
        public async Task ObterVencimentosAsync_DataInicioMenorQueDataFim_RetornaListaDeFinanceirosNoPeriodo()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now.AddDays(-1);
            DateTime dataFim = DateTime.Now.AddDays(1);
            var financeirosEsperados = new List<Financeiro>
            {
                new Financeiro { Id = 1, Descricao = "Teste1", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now }
            };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeirosEsperados);

            // Act
            var financeirosObtidos = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.Equal(financeirosEsperados, financeirosObtidos);
        }

        [Fact]
        public async Task ObterVencimentosAsync_DataInicioMaiorQueDataFim_LancaArgumentException()
        {
            // Arrange
            DateTime dataInicio = DateTime.Now.AddDays(1);
            DateTime dataFim = DateTime.Now.AddDays(-1);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterVencimentosAsync(dataInicio, dataFim));
        }

        [Fact]
        public async Task AdicionarAsync_DadosValidos_AdicionaFinanceiroERetornaTrue()
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
        [InlineData(null, 100, "Entrada", true)]
        [InlineData("", 100, "Entrada", true)]
        [InlineData("   ", 100, "Entrada", true)]
        [InlineData("Teste", 0, "Entrada", true)]
        [InlineData("Teste", -100, "Entrada", true)]
        [InlineData("Teste", 100, "Invalido", false)]
        public async Task AdicionarAsync_DadosInvalidos_LancaArgumentException(string descricao, decimal valor, string tipoFinanceiro, bool tipoValido)
        {
            // Arrange
            DateTime dtVencimento = DateTime.Now;

            // Act & Assert
            if (descricao == null || string.IsNullOrWhiteSpace(descricao))
                await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            else if (valor <= 0)
                await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            else if (!tipoValido)
                await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));

        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_FinanceiroExiste_AtualizaStatusERetornaTrue()
        {
            // Arrange
            int financeiroId = 1;
            bool baixado = true;
            var financeiroExistente = new Financeiro { Id = financeiroId, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now, Baixado = false };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync(financeiroExistente);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(financeiroId, baixado);

            // Assert
            Assert.True(resultado);
            Assert.Equal(baixado, financeiroExistente.Baixado);
            Assert.NotNull(financeiroExistente.DtBaixa);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_FinanceiroNaoExiste_RetornaFalse()
        {
            // Arrange
            int financeiroId = 1;
            bool baixado = true;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(financeiroId, baixado);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_FinanceiroExiste_AtualizaDataVencimentoERetornaTrue()
        {
            // Arrange
            int financeiroId = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(7);
            var financeiroExistente = new Financeiro { Id = financeiroId, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync(financeiroExistente);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(financeiroId, novaDataVencimento);

            // Assert
            Assert.True(resultado);
            Assert.Equal(novaDataVencimento.Date, financeiroExistente.DtVencimento.Date);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AlterarDataVencimentoAsync_FinanceiroNaoExiste_RetornaFalse()
        {
            // Arrange
            int financeiroId = 1;
            DateTime novaDataVencimento = DateTime.Now.AddDays(7);
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(financeiroId, novaDataVencimento);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_FinanceiroExiste_AtualizaFinanceiroERetornaTrue()
        {
            // Arrange
            int financeiroId = 1;
            string novaDescricao = "Teste Atualizado";
            decimal novoValor = 200;
            string novoTipoFinanceiro = "Saída";
            DateTime novaDtVencimento = DateTime.Now.AddDays(10);
            var financeiroExistente = new Financeiro { Id = financeiroId, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync(financeiroExistente);
            _financeiroRepositoryMock.Setup(repo => repo.AtualizarAsync(It.IsAny<Financeiro>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(financeiroId, novaDescricao, novoValor, novoTipoFinanceiro, novaDtVencimento);

            // Assert
            Assert.True(resultado);
            Assert.Equal(novaDescricao, financeiroExistente.Descricao);
            Assert.Equal(novoValor, financeiroExistente.Valor);
            Assert.Equal(novoTipoFinanceiro, financeiroExistente.TipoFinanceiro);
            Assert.Equal(novaDtVencimento, financeiroExistente.DtVencimento);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_FinanceiroNaoExiste_RetornaFalse()
        {
            // Arrange
            int financeiroId = 1;
            string novaDescricao = "Teste Atualizado";
            decimal novoValor = 200;
            string novoTipoFinanceiro = "Saída";
            DateTime novaDtVencimento = DateTime.Now.AddDays(10);

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(financeiroId, novaDescricao, novoValor, novoTipoFinanceiro, novaDtVencimento);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Theory]
        [InlineData(null, 100, "Entrada", true)]
        [InlineData("", 100, "Entrada", true)]
        [InlineData("   ", 100, "Entrada", true)]
        [InlineData("Teste", 0, "Entrada", true)]
        [InlineData("Teste", -100, "Entrada", true)]
        [InlineData("Teste", 100, "Invalido", false)]
        public async Task AtualizarAsync_DadosInvalidos_LancaArgumentException(string descricao, decimal valor, string tipoFinanceiro, bool tipoValido)
        {
            // Arrange
             int financeiroId = 1;
            DateTime dtVencimento = DateTime.Now;

             _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync(new Financeiro { Id = financeiroId, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now });

            // Act & Assert
            if (descricao == null || string.IsNullOrWhiteSpace(descricao))
                await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(financeiroId, descricao, valor, tipoFinanceiro, dtVencimento));
            else if (valor <= 0)
                await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(financeiroId, descricao, valor, tipoFinanceiro, dtVencimento));
            else if (!tipoValido)
                await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(financeiroId, descricao, valor, tipoFinanceiro, dtVencimento));

        }

        [Fact]
        public async Task RemoverAsync_FinanceiroExiste_RemoveFinanceiroERetornaTrue()
        {
            // Arrange
            int financeiroId = 1;
            var financeiroExistente = new Financeiro { Id = financeiroId, Descricao = "Teste", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync(financeiroExistente);
            _financeiroRepositoryMock.Setup(repo => repo.RemoverAsync(financeiroId)).Returns(Task.CompletedTask);

            // Act
            var resultado = await _financeiroService.RemoverAsync(financeiroId);

            // Assert
            Assert.True(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(financeiroId), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_FinanceiroNaoExiste_RetornaFalse()
        {
            // Arrange
            int financeiroId = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync((Financeiro)null);

            // Act
            var resultado = await _financeiroService.RemoverAsync(financeiroId);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.RemoverAsync(financeiroId), Times.Never);
        }
    }
}