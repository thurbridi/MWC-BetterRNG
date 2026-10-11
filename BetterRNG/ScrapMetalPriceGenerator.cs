namespace BetterRNG
{
    internal class ScrapMetalPriceGenerator : INoise1D
    {
        private readonly INoise1D _marketNoise;
        private readonly INoise1D _regimeNoise;
        private const float BasePrice = 0.96f;
        private const float PriceOffset = 0.07f;
        public float RegimeWeight { get; set; } = 1f;
        public float Sigma { get; set; } = 1f;

        internal ScrapMetalPriceGenerator(INoise1D marketNoise, INoise1D regimeNoise)
        {
            _marketNoise = marketNoise;
            _regimeNoise = regimeNoise;
        }

        public float Sample(float x)
        {
            float regimeTerm = RegimeWeight * (1f + _regimeNoise.Sample(x));
            float marketTerm = 1f + Sigma * _marketNoise.Sample(x);

            return PriceOffset + BasePrice * regimeTerm * marketTerm;
        }
    }
}
