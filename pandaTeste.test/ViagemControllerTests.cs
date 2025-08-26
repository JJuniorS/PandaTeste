using Microsoft.AspNetCore.Mvc;
using Moq;
using pandaTeste.api.Application.Interfaces;
using pandaTeste.api.Controllers;
using pandaTeste.api.Domain.Entities;

namespace pandaTeste.test
{
    public class ViagemControllerTests
    {
        private readonly Mock<IViagemService> _viagemServiceMock;

        // Declara a classe a ser testada (SUT - System Under Test)
        private readonly ViagensController _sut;

        public ViagemControllerTests()
        {
            // 1. Arrange: Instancia o mock.
            _viagemServiceMock = new Mock<IViagemService>();

            // 2. Act: Instancia o SUT, injetando a "versão simulada" do mock (.Object).
            _sut = new ViagensController(_viagemServiceMock.Object);
        }

        [Fact]
        public async Task GetViagensAgendadas_ComViagensExistentes_DeveRetornarOkComListaDeViagens()
        {
            // Arrange
            // Prepara uma lista de viagens de teste que o serviço mockado irá retornar.
            var viagensEsperadas = new List<Viagem>
            {
                new Viagem { Cliente = "Cliente A", Destino = "São Paulo", Preco = 250.0m, DtViagem = System.DateTime.Now, Id = 1, Orcamento = 300 }
            };

            // Configura o mock do serviço para retornar a lista de viagens esperada
            // quando o método "ObterViagensAgendadasAsync" for chamado.
            _viagemServiceMock.Setup(service => service.ObterViagensAgendadasAsync())
                                 .ReturnsAsync(viagensEsperadas);

            // Act
            // Executa a ação a ser testada, chamando o método do controlador.
            var resultado = await _sut.Get();

            // Assert
            // 1. Verifica se o tipo de retorno do resultado está correto (OkObjectResult).
            var okResult = Assert.IsType<OkObjectResult>(resultado);

            // 2. Verifica se a lista de viagens retornada não é nula.
            var listaDeViagens = Assert.IsType<List<Viagem>>(okResult.Value);

            // 3. Verifica se a quantidade de viagens é a esperada.
            Assert.Equal(viagensEsperadas.Count, listaDeViagens.Count);

            // Verifica se o método do mock foi realmente chamado UMA vez.
            _viagemServiceMock.Verify(service => service.ObterViagensAgendadasAsync(), Times.Once);
        }

        [Fact]
        public async Task GetViagensAgendadas_ComServicoRetornandoListaVazia_DeveRetornarOkComListaVazia()
        {
            // Arrange
            // Prepara o mock do serviço para retornar uma lista vazia, simulando um cenário sem dados.
            var viagensVazias = new List<Viagem>();
            _viagemServiceMock.Setup(service => service.ObterViagensAgendadasAsync())
                                 .ReturnsAsync(viagensVazias);

            // Act
            // Executa a ação a ser testada.
            var resultado = await _sut.Get();

            // Assert
            // Verifica se o resultado é um OkObjectResult e se a lista retornada está vazia.
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var listaDeViagens = Assert.IsType<List<Viagem>>(okResult.Value);
            Assert.Empty(listaDeViagens);
        }

        [Fact]
        public async Task GetViagensAgendadas_ComExcecaoNoServico_DeveRetornarStatusCode500()
        {
            // Arrange
            // Prepara o mock do serviço para lançar uma exceção, simulando uma falha.
            _viagemServiceMock.Setup(service => service.ObterViagensAgendadasAsync())
                                 .ThrowsAsync(new System.Exception("Erro de conexão."));

            // Act
            // Executa a ação a ser testada.
            var resultado = await _sut.Get();

            // Assert
            // Verifica se o resultado é um StatusCodeResult com o código 500 (Internal Server Error).
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, statusCodeResult.StatusCode);

            // Verifica se o método do mock foi chamado uma vez.
            _viagemServiceMock.Verify(service => service.ObterViagensAgendadasAsync(), Times.Once);
        }
    }
}
