using System;
using MSCCoreLibrary;
using MSCLoader;

namespace BetterRNG
{
    /// <summary>
    /// This class substitutes Forecast.Logic FSM with a custom weather generation logic that uses a ValueNoise-based temperature generator.
    /// </summary>
    internal class WeatherPatcher
    {
        private INoise1D _temperatureGenerator;
        private SnowForecastGenerator _snowForecastGenerator;
        private readonly ForecastAdapter _forecast;
        private readonly WeatherAdapter _weather;
        private readonly TimeAdapter _time;
        private readonly TVAdapter _tv;

        private const int weekLength = 7;

        internal WeatherPatcher(INoise1D temperatureFunction, SnowForecastGenerator snowForecastGenerator)
        {
            _temperatureGenerator = temperatureFunction;
            _snowForecastGenerator = snowForecastGenerator;
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
            ModConsole.Log("Scheduled weekly weather generation.");

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
                bool snowing = _snowForecastGenerator.GetSnowForTemp(temp, daysPassed + i);
                _forecast.SetWeeklySnow(i, snowing);
            }
        }

        private void UpdateTemperature()
        {
            _weather.SetAmbientTemperature(_temperatureGenerator.Sample(_time.GetDayFraction()));
        }
    }

    /// <summary>
    /// Substitutes vanilla scrap metal price Random Walk method with Value Noise. 
    /// 
    /// Vanilla method is a mix of markov chain and random walk. 
    /// 90% chance to have a small change ([-0.15, 0.15]), 5% chance to have a large positive change ([0.2, 1.5] and a %5 chance to have a large negative change [-1.75, -0.5]. 
    /// Notice a bias towards negative changes.
    /// 
    /// Price is capped between 0.07 and 90.
    /// 
    /// The scrap metal price for the week is updated every business day at 10am.
    /// </summary>
    internal class ScrapMetalPricePatcher
    {
        private readonly ScrapMetalPriceAdapter _scrapMetalPriceAdapter;
        private readonly TimeAdapter _time;
        private readonly INoise1D _priceGenerator;

        internal ScrapMetalPricePatcher(INoise1D priceGenerator)
        {
            _scrapMetalPriceAdapter = new ScrapMetalPriceAdapter();
            _time = new TimeAdapter();
            _priceGenerator = priceGenerator;
        }

        public void Patch()
        {
            try
            {
                GeneratePriceHistory();
                ScheduleScrapMetalPriceUpdate();
                ModConsole.Log("Scheduled scrap metal price updates.");
                _scrapMetalPriceAdapter.SetLogicFsmActive(false);
                ModConsole.Log("Disabled vanilla scrap metal price logic.");
            }
            catch (Exception e)
            {
                ModConsole.Error($"Failed to patch scrap price logic. Error: {e}");
            }
        }

        private void ScheduleScrapMetalPriceUpdate()
        {
            TimeScheduler.ScheduleAction(
                hour: 10,
                minute: 0,
                action: UpdateScrapMetalPrice,
                day: GameTime.Days.Week
            );
        }

        private void GeneratePriceHistory()
        {
            int currentBusinessDay = _time.GetBusinessDaysPassed();

            for (int daysAgo = ScrapMetalPriceAdapter.historyLength - 1; daysAgo > 0; daysAgo--)
            {
                _scrapMetalPriceAdapter.PushPrice(GetPrice(currentBusinessDay - daysAgo));
            }

            _scrapMetalPriceAdapter.UpdatePrice(GetPrice(currentBusinessDay));
        }

        private void UpdateScrapMetalPrice()
        {
            _scrapMetalPriceAdapter.UpdatePrice(GetPrice(_time.GetBusinessDaysPassed()));
        }

        private float GetPrice(int businessDay)
        {
            return _priceGenerator.Sample(businessDay + ScrapMetalPriceAdapter.historyOffset);
        }
    }
}
