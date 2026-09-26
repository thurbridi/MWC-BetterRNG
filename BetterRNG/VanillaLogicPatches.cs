using MSCCoreLibrary;
using MSCLoader;
using UnityEngine;

namespace BetterRNG
{
    /// <summary>
    /// This class substitutes Forecast.Logic FSM with a custom weather generation logic that uses a ValueNoise-based temperature generator.
    /// </summary>
    internal class WeatherPatcher
    {
        private INoise1D _temperatureGenerator;
        private readonly ForecastAdapter _forecast;
        private readonly WeatherAdapter _weather;
        private readonly TimeAdapter _time;
        private readonly TVAdapter _tv;

        private const int weekLength = 7;

        internal WeatherPatcher(INoise1D temperatureFunction)
        {
            _temperatureGenerator = temperatureFunction;
            _forecast = new ForecastAdapter();
            _weather = new WeatherAdapter();
            _time = new TimeAdapter();
            _tv = new TVAdapter();
        }

        public void Patch()
        {
            _forecast.SetLogicFsmActive(false);
            ModConsole.Log("Disabled vanilla weather logic.");

            ScheduleWeeklyWeatherGeneration();

            GenerateWeatherForThisWeek();

            _forecast.AddTemperatureBehaviour(UpdateTemperature);
        }

        /// <summary>
        /// Add a callback to the OnNextDay event to update the weather every week.
        /// </summary>
        private void ScheduleWeeklyWeatherGeneration()
        {
            GameTime.OnNextDay += (day) =>
            {
                // Runs on sunday -> monday transition.
                if (day == GameTime.Days.Monday)
                {
                    GenerateWeeklyWeather(_time.GetDaysPassed());
                }

                _tv.ResetTvNoise();

                int weekDay = _time.GetWeekDay();
                UpdateTemperature();
                _weather.SetSnowyDay(_forecast.GetWeeklySnow(weekDay));
                _forecast.SetSnowy(_forecast.GetWeeklySnow(weekDay));

            };
        }

        private void GenerateWeatherForThisWeek()
        {
            _tv.ResetTvNoise();

            int weekDay = _time.GetWeekDay();
            int currentWeekMondayDay = _time.GetDaysPassed() - weekDay;

            GenerateWeeklyWeather(currentWeekMondayDay);

            UpdateTemperature();
            _weather.SetSnowyDay(_forecast.GetWeeklySnow(weekDay));
            _forecast.SetSnowy(_forecast.GetWeeklySnow(weekDay));
        }

        private void GenerateWeeklyWeather(int daysPassed)
        {
            // Vanilla logic stores temps in an array with size 8 but leaves index 0 unused.
            for (int i = 0; i < weekLength; i++)
            {
                _forecast.SetWeeklyTemperature(i, _temperatureGenerator.Sample(daysPassed + i));
            }

            // Vanilla snow generation logic.
            for (int i = 0; i < weekLength; i++)
            {
                float temp = _forecast.GetWeeklyTemperature(i);
                _forecast.SetWeeklySnow(i, temp > -7f && Random.value < 0.65f);
            }
        }

        private void UpdateTemperature()
        {
            _weather.SetAmbientTemperature(_temperatureGenerator.Sample(_time.GetDayFraction()));
        }
    }
}
