using HutongGames.PlayMaker;
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
        private Transform _forecast, _clouds, _rain;
        private INoise1D _temperatureGenerator;
        private FsmFloat _ambientTemperature;
        private PlayMakerArrayListProxy _weekly_temps, _weekly_snow;
        private FsmInt _weekDay;
        private const int weekLength = 8; // it's Topless, don't ask

        internal WeatherPatcher()
        {
            _forecast = GameObject.Find("MAP").transform.Find("WEATHER/Forecast");
            _temperatureGenerator = new TemperatureGenerator();
            _ambientTemperature = FsmVariables.GlobalVariables.GetFsmFloat("AmbientTemperature");
            _weekly_temps = _forecast.gameObject.GetArrayListProxy("Weekly");
            _weekly_snow = _forecast.gameObject.GetArrayListProxy("Snow");
            _weekDay = FsmVariables.GlobalVariables.GetFsmInt("GlobalDay");
            _clouds = GameObject.Find("MAP").transform.Find("WEATHER/Clouds/CloudObjects");
            _rain = GameObject.Find("PLAYER").transform.Find("Rain");
        }

        public void Patch()
        {
            var forecastLogicFsm = _forecast.GetPlayMaker("Logic");
            forecastLogicFsm.enabled = false;

            GameTime.OnNextDay += (day) =>
            {
                // Runs on sunday -> monday transition.
                if (day == GameTime.Days.Monday)
                {
                    GenerateWeeklyWeather();
                }

                SetAmbientTemperature((float)_weekly_temps._arrayList[_weekDay.Value]);
                SetSnowyDay((bool)_weekly_snow._arrayList[_weekDay.Value]);
            };
        }

        private void SetAmbientTemperature(float value)
        {
            ModConsole.Log($"Setting ambient temperature to {value}");
            _ambientTemperature.Value = value;
        }

        private void SetSnowyDay(bool value)
        {
            _clouds.gameObject.SetActive(value);
            _rain.GetPlayMaker("Rain").FsmVariables.GetFsmBool("RainYes").Value = value;
        }

        private void GenerateWeeklyWeather()
        {
            int daysPassed = FsmVariables.GlobalVariables.GetFsmInt("DaysPassed").Value;


            // Vanilla logic stores temps in an array with size 8 but leaves index 0 unused.
            for (int i = 1; i < weekLength; i++)
            {
                _weekly_temps._arrayList[i] = _temperatureGenerator.Sample(daysPassed + i);
            }

            // Vanilla snow generation logic.
            for (int i = 1; i < weekLength; i++)
            {
                float temp = (float)_weekly_temps._arrayList[i];
                _weekly_snow._arrayList[i] = temp > -7f && Random.value < 0.35f;
            }
        }
    }
}
