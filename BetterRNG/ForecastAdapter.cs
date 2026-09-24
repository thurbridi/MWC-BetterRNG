using MSCLoader;
using UnityEngine;

namespace BetterRNG
{
    /// <summary>
    /// Class for interacting with vanilla `GameObject` `Forecast`. This GameObject holds weekly temperature and snow data, as well as the weather logic FSM. 
    /// </summary>
    public class ForecastAdapter
    {
        private readonly PlayMakerFSM _forecastLogicFsm;
        private readonly PlayMakerArrayListProxy _weekly_temps, _weekly_snow;

        public ForecastAdapter()
        {
            var forecast = GameObject.Find("MAP").transform.Find("WEATHER/Forecast");
            _forecastLogicFsm = forecast.GetPlayMaker("Logic");
            _weekly_temps = forecast.gameObject.GetArrayListProxy("Weekly");
            _weekly_snow = forecast.gameObject.GetArrayListProxy("Snow");
        }

        public void SetLogicFsmActive(bool isActive)
        {
            _forecastLogicFsm.enabled = isActive;
        }

        public float GetWeeklyTemperature(int index)
        {
            int oneBasedIndex = ToWeekIndex(index);
            return (float)_weekly_temps._arrayList[oneBasedIndex];
        }

        public void SetWeeklyTemperature(int index, float temperature)
        {
            int oneBasedIndex = ToWeekIndex(index);
            _weekly_temps._arrayList[oneBasedIndex] = temperature;
        }

        public bool GetWeeklySnow(int index)
        {
            int oneBasedIndex = ToWeekIndex(index);
            return (bool)_weekly_snow._arrayList[oneBasedIndex];
        }

        public void SetWeeklySnow(int index, bool isSnowy)
        {
            int oneBasedIndex = ToWeekIndex(index);
            _weekly_snow._arrayList[oneBasedIndex] = isSnowy;
        }

        private int ToWeekIndex(int index)
        {
            ValidateIndex(index);
            return index + 1; // Adjust for 1-based indexing in the array list
        }

        /// <summary>
        /// Validates that the provided index is within the valid range for weekly data [0,6]. Throws an exception if the index is out of range.
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= 7)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index), "Index must be in the range 0-6.");
            }
        }
    }
}
