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
        private INoise1D _temperatureGenerator;

        private readonly PlayMakerFSM _forecastLogicFsm;
        private readonly GameObject _clouds;
        private readonly FsmFloat _ambientTemperature;
        private readonly FsmBool _rain;
        private readonly PlayMakerArrayListProxy _weekly_temps, _weekly_snow;
        private readonly FsmInt _weekDay, _daysPassed;

        private const int weekLength = 8; // it's Topless, don't ask

        internal WeatherPatcher(INoise1D temperatureFunction)
        {
            _temperatureGenerator = temperatureFunction;

            _ambientTemperature = FsmVariables.GlobalVariables.GetFsmFloat("AmbientTemperature");
            _weekDay = FsmVariables.GlobalVariables.GetFsmInt("GlobalDay");
            _daysPassed = FsmVariables.GlobalVariables.GetFsmInt("DaysPassed");

            var forecast = GameObject.Find("MAP").transform.Find("WEATHER/Forecast");
            _forecastLogicFsm = forecast.GetPlayMaker("Logic");
            _weekly_temps = forecast.gameObject.GetArrayListProxy("Weekly");
            _weekly_snow = forecast.gameObject.GetArrayListProxy("Snow");

            _clouds = GameObject.Find("MAP").transform.Find("WEATHER/Clouds/CloudObjects").gameObject;
            _rain = GameObject.Find("PLAYER").transform.Find("Rain").GetPlayMaker("Rain").FsmVariables.GetFsmBool("RainYes");
        }

        public void Patch()
        {
            _forecastLogicFsm.enabled = false;
            ModConsole.Log("Disabled vanilla weather logic.");

            // Add a callback to the OnNextDay event to update the weather every week.
            GameTime.OnNextDay += (day) =>
            {
                // Runs on sunday -> monday transition.
                if (day == GameTime.Days.Monday)
                {
                    GenerateWeeklyWeather(_daysPassed.Value);
                }

                int weekDay = _weekDay.Value;
                SetAmbientTemperature((float)_weekly_temps._arrayList[weekDay]);
                SetSnowyDay((bool)_weekly_snow._arrayList[weekDay]);
            };

            GenerateWeatherForThisWeek();
        }

        private void SetAmbientTemperature(float value)
        {
            ModConsole.Log($"Setting ambient temperature to {value}");
            _ambientTemperature.Value = value;
        }

        private void SetSnowyDay(bool value)
        {
            _clouds.SetActive(value);
            _rain.Value = value;
        }

        private void GenerateWeatherForThisWeek()
        {
            int weekDay = _weekDay.Value;
            int currentWeekMondayDay = (_daysPassed.Value + 1) - weekDay;

            GenerateWeeklyWeather(currentWeekMondayDay);

            SetAmbientTemperature((float)_weekly_temps._arrayList[weekDay]);
            SetSnowyDay((bool)_weekly_snow._arrayList[weekDay]);
        }

        private void GenerateWeeklyWeather(int daysPassed)
        {
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
