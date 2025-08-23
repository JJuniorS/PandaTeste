using pandaTeste.api.Controllers;

namespace pandaTeste.test
{
    public class WeatherForecastTest
    {
        [Fact]
        public void Get_ReturnsCorrectNumberOfForecasts()
        {
            // Arrange
            var controller = new WeatherForecastController();

            // Act
            var result = controller.Get().ToList(); // Convertendo IEnumerable para List

            // Assert
            Assert.Equal(5, result.Count); // O WeatherForecastController padrão retorna 5 itens
        }

        [Fact]
        public void Get_ReturnsValidTemperatureRange()
        {
            // Arrange
            var controller = new WeatherForecastController();

            // Act
            var result = controller.Get();

            // Assert
            foreach (var forecast in result)
            {
                Assert.InRange(forecast.TemperatureC, -20, 55); // Limites padrão do template
            }
        }
    }
}