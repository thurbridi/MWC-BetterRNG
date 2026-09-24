using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace BetterRNG
{
    /// <summary>
    /// Class responsible for interacting with the vanilla weather system in the game. Namely the ambient temperature and snow effects.
    /// </summary>
    public class WeatherAdapter
    {

        private readonly FsmFloat _ambientTemperature;
        private readonly GameObject _clouds;
        private readonly FsmBool _rain;

        public WeatherAdapter()
        {
            _ambientTemperature = FsmVariables.GlobalVariables.GetFsmFloat("AmbientTemperature");

            _clouds = GameObject.Find("MAP").transform.Find("WEATHER/Clouds/CloudObjects").gameObject;
            _rain = GameObject.Find("PLAYER").transform.Find("Rain").GetPlayMaker("Rain").FsmVariables.GetFsmBool("RainYes"); // name says rain but is only snow
        }

        public void SetAmbientTemperature(float value)
        {
            _ambientTemperature.Value = value;
        }

        public float GetAmbientTemperature()
        {
            return _ambientTemperature.Value;
        }

        public void SetSnowyDay(bool isSnowy)
        {
            _clouds.SetActive(isSnowy);
            _rain.Value = isSnowy;
        }
    }
}
