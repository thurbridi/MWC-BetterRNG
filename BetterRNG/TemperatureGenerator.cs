using System;

namespace BetterRNG
{
    internal class TemperatureGenerator : INoise1D
    {
        private INoise1D _noise_src, _noise_a, _noise_b, _noise_c, _noise_d;

        internal TemperatureGenerator(Int32 seed = 777)
        {
            _noise_src = new ValueNoise(size: 64, seed);
            _noise_a = new NoiseFunction1D(_noise_src, 0.2f, 0.4f, 0f);
            _noise_b = new NoiseFunction1D(_noise_src, 0.4f, 0.3f, 0f);
            _noise_c = new NoiseFunction1D(_noise_src, 0.8f, 0.2f, 0f);
            _noise_d = new NoiseFunction1D(_noise_src, 1.6f, 0.1f, 0f);
        }

        public float Sample(float x)
        {
            return (_noise_a.Sample(x) + _noise_b.Sample(x) + _noise_c.Sample(x) + _noise_d.Sample(x)) * 20 - 22;
        }
    }
}
