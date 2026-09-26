using System;

namespace BetterRNG
{
    public interface INoise1D
    {
        public float Sample(float x);
    }

    public class NoiseFunction1D : INoise1D
    {
        private readonly INoise1D _noise_src;
        private readonly float _frequency, _amplitude, _offset;

        public NoiseFunction1D(INoise1D rnd_source, float frequency, float amplitude, float offset)
        {
            _noise_src = rnd_source;
            _frequency = frequency;
            _amplitude = amplitude;
            _offset = offset;
        }

        public float Sample(float x)
        {
            return _noise_src.Sample(x * _frequency + _offset) * _amplitude;
        }
    }

    /// <summary>
    /// 1D Value Noise generator. Sample function is periodic with period equal to the size of the _values array.
    /// 
    /// Reference: https://www.scratchapixel.com/lessons/procedural-generation-virtual-worlds/procedural-patterns-noise-part-1//creating-simple-1D-noise.html
    /// </summary>
    public class ValueNoise : INoise1D
    {

        private readonly uint _size = 10;
        private float[] _values;

        public ValueNoise(uint size, Int32 seed)
        {
            _size = size;
            _values = new float[_size];

            var rng = new Random(seed);

            InitializeValues(rng);
        }

        public ValueNoise(Int32 seed)
        {
            _values = new float[_size];

            var rng = new Random(seed);

            InitializeValues(rng);
        }

        private void InitializeValues(Random rng)
        {
            for (int i = 0; i < _size; i++)
            {
                _values[i] = (float)rng.NextDouble() * 2 - 1;
            }
        }

        /// <summary>
        /// Samples the value noise at a given x coordinate. The result is interpolated between the two nearest values in the _values array.
        /// </summary>
        /// <param name="x">Coordinate to be sampled. If outsize the range given by the size of _values, x will be wrapped thus making this function periodic.</param>
        /// <returns>A float in the interval [-1,1)</returns>
        public float Sample(float x)
        {
            float _x = x % _size;

            int x0 = (int)_x;
            int x1 = (x0 + 1) % (int)_size;

            float v0 = _values[x0];
            float v1 = _values[x1];

            float t = _x - x0;

            t = SmoothStep(t);

            return Lerp(v0, v1, t);
        }

        private static float SmoothStep(float x)
        {
            return x * x * (3 - 2 * x);
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }
    }
}
