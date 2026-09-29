using System;

namespace BetterRNG
{
    internal class TemperatureGenerator : INoise1D
    {
        public float Amplitude { get; set; } = 20f;
        public float Offset { get; set; } = -22f;
        private readonly INoise1D[] _noiseLayers;
        private readonly INoise1D _coldNoise;
        private readonly float _warmupPeriodDays;
        private readonly float _startingTemperature;
        public float ColdAmplitude { get; set; } = 15f;

        internal TemperatureGenerator(INoise1D[] noiseLayers, INoise1D coldNoise = null, float warmupPeriodDays = 0f, float startingTemperature = 0f)
        {
            if (warmupPeriodDays < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(warmupPeriodDays), "Warmup period must be non-negative.");
            }

            _noiseLayers = noiseLayers;
            _coldNoise = coldNoise;
            _warmupPeriodDays = warmupPeriodDays;
            _startingTemperature = startingTemperature;
        }

        public float Sample(float x)
        {
            float sample = 0f;
            foreach (INoise1D noiseLayer in _noiseLayers)
            {
                sample += noiseLayer.Sample(x);
            }

            sample = sample * Amplitude + Offset;

            if (_coldNoise != null)
            {
                float cold = _coldNoise.Sample(x);
                if (cold < 0f)
                {
                    sample += cold * ColdAmplitude;
                }
            }

            float weight = _warmupPeriodDays == 0f ? 1f : x / _warmupPeriodDays;
            weight = NoiseUtils.Clamp(weight, 0f, 1f);
            weight = NoiseUtils.SmoothStep(weight);
            sample = NoiseUtils.Lerp(_startingTemperature, sample, weight);

            return sample;
        }
    }
}
