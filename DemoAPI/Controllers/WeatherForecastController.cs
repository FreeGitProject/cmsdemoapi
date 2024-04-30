using Microsoft.AspNetCore.Mvc;

namespace DemoAPI.Controllers
{

    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/WeatherForecast")]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _logger.LogInformation("WeatherForecast controller called ");
        }
        /// <summary>
        /// WeatherForecast
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetWeatherForecast")]
      //  [Route("all")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation("WeatherForecast get method Starting.");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
