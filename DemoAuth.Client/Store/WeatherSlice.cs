using DemoAuth.Client.Models;
using Fluxor;

namespace DemoAuth.Client.AppState.Weather
{
    // ********************
    // State
    // ********************
    [FeatureState]
    public record WeatherForecastState
    {
        public WeatherForecast[]? Forecasts { get; init; } = null;
    }

    // ********************
    // Reducers
    // ********************
    public static class Reducers
    {
        [ReducerMethod]
        public static WeatherForecastState WeatherForecastsRetrievedReducer(WeatherForecastState state, WeatherForecastRetrieved action)
        {
            return state with { Forecasts = action.Forecasts };
        }
    }

    // ********************
    // Effects
    // ********************
    public class Effects
    {

        [EffectMethod(typeof(WeatherForecastFetched))]
        public async Task WeatherForecastFetchedReducer(IDispatcher dispatcher)
        {
            // Simulate asynchronous loading to demonstrate streaming rendering
            await Task.Delay(1000);

            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var summaries = new[] {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            }).ToArray();

            dispatcher.Dispatch(new WeatherForecastRetrieved(forecasts));
        }
    }

    // ********************
    // Actions
    // ********************
    public record WeatherForecastFetched();
    public record WeatherForecastRetrieved(WeatherForecast[]? Forecasts);
}