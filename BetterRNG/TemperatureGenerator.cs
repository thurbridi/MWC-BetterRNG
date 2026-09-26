namespace BetterRNG
{
    internal class TemperatureGenerator : INoise1D
    {
        public float Amplitude { get; set; } = 20f;
        public float Offset { get; set; } = -22f;
        private readonly INoise1D[] _noiseLayers;
        private readonly INoise1D _coldNoise;
        public float ColdAmplitude { get; set; } = 15f;

        internal TemperatureGenerator(INoise1D[] noiseLayers, INoise1D coldNoise = null)
        {
            _noiseLayers = noiseLayers;
            _coldNoise = coldNoise;
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

            return sample;
        }
    }
}
