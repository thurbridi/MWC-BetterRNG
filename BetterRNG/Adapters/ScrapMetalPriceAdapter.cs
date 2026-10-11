using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace BetterRNG
{
    internal class ScrapMetalPriceAdapter
    {
        private readonly PlayMakerFSM _logic;
        private readonly PlayMakerArrayListProxy _prices;
        private readonly FsmFloat _change;
        private readonly FsmFloat _scrapPriceMKkg;
        public const int historyLength = 10;
        public const int historyOffset = historyLength - 1;


        public ScrapMetalPriceAdapter()
        {
            var scrapMetalPrice = GameObject.Find("Systems").transform.Find("ScrapMetalPrice");
            _logic = scrapMetalPrice.GetPlayMaker("Logic");
            _prices = scrapMetalPrice.gameObject.GetArrayListProxy("Prices");
            _change = _logic.FsmVariables.GetFsmFloat("Change");
            _scrapPriceMKkg = _logic.FsmVariables.GetFsmFloat("ScrapPriceMKkg");
        }

        public void SetLogicFsmActive(bool isActive)
        {
            _logic.enabled = isActive;
        }

        public void ClearPrices()
        {
            _prices._arrayList.Clear();
        }

        public float Change
        {
            get { return _change.Value; }
            set { _change.Value = value; }
        }

        public float ScrapPriceMKkg
        {
            get { return _scrapPriceMKkg.Value; }
            set { _scrapPriceMKkg.Value = value; }
        }

        /// <summary>
        /// Adds a new value to the list of scrap metal prices. The list behaves like a queue, so the oldest value will be removed if the list is full.
        /// </summary>
        /// <param name="price">The new scrap metal price to add to the list.</param>
        public void PushPrice(float price)
        {
            _prices._arrayList.Insert(0, price);
            if (_prices._arrayList.Count > historyLength)
            {
                _prices._arrayList.RemoveAt(_prices._arrayList.Count - 1);
            }
        }

        public void UpdatePrice(float price)
        {
            PushPrice(price);
            ScrapPriceMKkg = price;

            if (_prices._arrayList.Count > 1)
            {
                float previousPrice = (float)_prices._arrayList[1];
                Change = previousPrice == 0f ? 0f : (price - previousPrice) / previousPrice * 100f;
            }
            else
            {
                Change = 0f;
            }
        }
    }
}
