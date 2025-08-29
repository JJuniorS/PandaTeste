using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Models;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pandaTeste.Tests
{
    public class FinanceiroServiceTests
    {
        private readonly Mock<IFinanceiroRepository> _financeiroRepositoryMock;
        private readonly IFinanceiroService _financeiroService;

        public FinanceiroServiceTests()
        {
            // Arrange: Inicializa os mocks e o serviço antes de cada teste
            _financeiroRepositoryMock = new Mock<IFinanceiroRepository>();
            _financeiroService = new FinanceiroService(_financeiroRepositoryMock.Object);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaFinanceiro_QuandoIdExiste()
        {
            // Arrange
            var financeiroId = 1;
            var financeiroMock = new Financeiro { Id = financeiroId, Descricao = "Teste" };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync(financeiroMock);

            // Act
            var financeiroRetornado = await _financeiroService.ObterPorIdAsync(financeiroId);

            // Assert
            Assert.NotNull(financeiroRetornado);
            Assert.Equal(financeiroId, financeiroRetornado.Id);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaNull_QuandoIdNaoExiste()
        {
            // Arrange
            var financeiroId = 1;
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(financeiroId)).ReturnsAsync(null as Financeiro);

            // Act
            var financeiroRetornado = await _financeiroService.ObterPorIdAsync(financeiroId);

            // Assert
            Assert.Null(financeiroRetornado);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaDeFinanceiros()
        {
            // Arrange
            var financeirosMock = new List<Financeiro> { new Financeiro { Id = 1, Descricao = "Teste1" }, new Financeiro { Id = 2, Descricao = "Teste2" } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(financeirosMock);

            // Act
            var financeirosRetornados = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(financeirosRetornados);
            Assert.NotEmpty(financeirosRetornados);
            Assert.Equal(2, ((List<Financeiro>)financeirosRetornados).Count);
        }

        [Fact]
        public async Task ObterTodosAsync_RetornaListaVazia_QuandoNaoHaFinanceiros()
        {
            // Arrange
            _financeiroRepositoryMock.Setup(repo => repo.ObterTodosAsync()).ReturnsAsync(new List<Financeiro>());

            // Act
            var financeirosRetornados = await _financeiroService.ObterTodosAsync();

            // Assert
            Assert.NotNull(financeirosRetornados);
            Assert.Empty(financeirosRetornados);
        }

        [Theory]
        [InlineData("Entrada")]
        [InlineData("Saída")]
        public async Task ObterPorTipoAsync_RetornaFinanceiros_QuandoTipoValido(string tipo)
        {
            // Arrange
            var financeirosMock = new List<Financeiro> { new Financeiro { Id = 1, TipoFinanceiro = tipo } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorTipoAsync(tipo)).ReturnsAsync(financeirosMock);

            // Act
            var financeirosRetornados = await _financeiroService.ObterPorTipoAsync(tipo);

            // Assert
            Assert.NotNull(financeirosRetornados);
            Assert.NotEmpty(financeirosRetornados);
            Assert.Equal(tipo, ((List<Financeiro>)financeirosRetornados)[0].TipoFinanceiro);
        }

        [Theory]
        [InlineData("Invalido")]
        [InlineData("")]
        [InlineData(null)]
        public async Task ObterPorTipoAsync_LancaExcecao_QuandoTipoInvalido(string tipo)
        {
            // Arrange

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.ObterPorTipoAsync(tipo));
        }

        [Fact]
        public async Task ObterPorStatusAsync_RetornaFinanceiros_QuandoStatusExiste()
        {
            // Arrange
            var status = true;
            var financeirosMock = new List<Financeiro> { new Financeiro { Id = 1, Baixado = status } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorStatusAsync(status)).ReturnsAsync(financeirosMock);

            // Act
            var financeirosRetornados = await _financeiroService.ObterPorStatusAsync(status);

            // Assert
            Assert.NotNull(financeirosRetornados);
            Assert.NotEmpty(financeirosRetornados);
            Assert.Equal(status, ((List<Financeiro>)financeirosRetornados)[0].Baixado);
        }

        [Fact]
        public async Task ObterVencimentosAsync_RetornaFinanceiros_QuandoDataValida()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddDays(-1);
            var dataFim = DateTime.Now.AddDays(1);
            var financeirosMock = new List<Financeiro> { new Financeiro { Id = 1, DtVencimento = DateTime.Now } };
            _financeiroRepositoryMock.Setup(repo => repo.ObterVencimentosAsync(dataInicio, dataFim)).ReturnsAsync(financeirosMock);

            // Act
            var financeirosRetornados = await _financeiroService.ObterVencimentosAsync(dataInicio, dataFim);

            // Assert
            Assert.NotNull(financeirosRetornados);
            Assert.NotEmpty(financeirosRetornados);
        }

        [Fact]
        public async Task ObterVencimentosAsync_LancaExcecao_QuandoDataInvalida()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddDays(1);
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
        [InlineData("", 100, "Entrada", "Descricao é obrigatória")]
        [InlineData(null, 100, "Entrada", "Descricao é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", -1, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 100, "Invalido", "Tipo financeiro inválido")]
        public async Task AdicionarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            var dtVencimento = DateTime.Now;

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AdicionarAsync(descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, ex.Message);

            _financeiroRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Financeiro>()), Times.Never);
        }


        [Fact]
        public async Task AlterarStatusBaixadoAsync_AlteraStatus_QuandoFinanceiroExiste()
        {
            // Arrange
            var id = 1;
            var baixado = true;
            var financeiroMock = new Financeiro { Id = id, Descricao = "Teste", Baixado = false };

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroMock);

            // Act
            var resultado = await _financeiroService.AlterarStatusBaixadoAsync(id, baixado);

            // Assert
            Assert.True(resultado);
            Assert.Equal(baixado, financeiroMock.Baixado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusBaixadoAsync_NaoAlteraStatus_QuandoFinanceiroNaoExiste()
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
            var financeiroMock = new Financeiro { Id = id, DtVencimento = DateTime.Now };

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroMock);

            // Act
            var resultado = await _financeiroService.AlterarDataVencimentoAsync(id, novaDataVencimento);

            // Assert
            Assert.True(resultado);
            Assert.Equal(novaDataVencimento.Date, financeiroMock.DtVencimento.Date);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
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
        public async Task AtualizarAsync_AtualizaFinanceiro_QuandoDadosValidosEFinanceiroExiste()
        {
            // Arrange
            var id = 1;
            var descricao = "Nova Descrição";
            var valor = 200;
            var tipoFinanceiro = "Saída";
            var dtVencimento = DateTime.Now.AddDays(5);
            var financeiroMock = new Financeiro { Id = id, Descricao = "Descricao Antiga", Valor = 100, TipoFinanceiro = "Entrada", DtVencimento = DateTime.Now };

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroMock);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.True(resultado);
            Assert.Equal(descricao, financeiroMock.Descricao);
            Assert.Equal(valor, financeiroMock.Valor);
            Assert.Equal(tipoFinanceiro, financeiroMock.TipoFinanceiro);
            Assert.Equal(dtVencimento.Date, financeiroMock.DtVencimento.Date);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_NaoAtualizaFinanceiro_QuandoFinanceiroNaoExiste()
        {
            // Arrange
            var id = 1;
            var descricao = "Nova Descrição";
            var valor = 200;
            var tipoFinanceiro = "Saída";
            var dtVencimento = DateTime.Now.AddDays(5);

            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(null as Financeiro);

            // Act
            var resultado = await _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento);

            // Assert
            Assert.False(resultado);
            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }

        [Theory]
        [InlineData("", 100, "Entrada", "Descricao é obrigatória")]
        [InlineData(null, 100, "Entrada", "Descricao é obrigatória")]
        [InlineData("Teste", 0, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", -1, "Entrada", "Valor deve ser maior que zero")]
        [InlineData("Teste", 100, "Invalido", "Tipo financeiro inválido")]
        public async Task AtualizarAsync_LancaExcecao_QuandoDadosInvalidos(string descricao, decimal valor, string tipoFinanceiro, string mensagemErro)
        {
            // Arrange
            var id = 1;
            var dtVencimento = DateTime.Now;

             _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(new Financeiro{ Id = 1 }); // Retornando um objeto válido para passar na primeira validação

            // Act & Assert
             var ex = await Assert.ThrowsAsync<ArgumentException>(() => _financeiroService.AtualizarAsync(id, descricao, valor, tipoFinanceiro, dtVencimento));
            Assert.Equal(mensagemErro, ex.Message);

            _financeiroRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Financeiro>()), Times.Never);
        }



        [Fact]
        public async Task RemoverAsync_RemoveFinanceiro_QuandoFinanceiroExiste()
        {
            // Arrange
            var id = 1;
            var financeiroMock = new Financeiro { Id = id };
            _financeiroRepositoryMock.Setup(repo => repo.ObterPorIdAsync(id)).ReturnsAsync(financeiroMock);

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