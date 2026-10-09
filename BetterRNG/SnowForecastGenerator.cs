namespace BetterRNG
{
    internal class SnowForecastGenerator
    {
        public float SnowChance { get; set; } = 0.65f;
        public float TemperatureThreshold { get; set; } = -7f;
        private INoise1D _rngSource;


        public SnowForecastGenerator(INoise1D rngSource)
        {
            _rngSource = rngSource;
        }

        public bool GetSnowForTemp(float temperature, float day)
        {
            return temperature > TemperatureThreshold && NoiseTransform(_rngSource.Sample(day)) < SnowChance;
        }

        private float NoiseTransform(float value)
        {
            return (value + 1f) / 2f;
        }
    }
}
