using System;

namespace BetterRNG
{
    internal class SnowForecastGenerator
    {
        private Random _rngSource;

        public SnowForecastGenerator(Random rngSource)
        {
            _rngSource = rngSource;
        }

        public bool GetSnowForTemp(float temperature)
        {
            return temperature > -7f && (float)_rngSource.NextDouble() < 0.65f;
        }
    }
}
