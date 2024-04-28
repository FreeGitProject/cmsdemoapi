namespace DemoAPI
{
  
    public class WeatherForecast
    {
        /// <summary>
        /// Temperature Date
        /// </summary>
        public DateOnly Date { get; set; }
        /// <summary>
        /// Temperature TemperatureC
        /// </summary>
        public int TemperatureC { get; set; }
        /// <summary>
        /// Temperature TemperatureF
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        /// <summary>
        /// Temperature Summary
        /// </summary>
        public string? Summary { get; set; }
    }
}
