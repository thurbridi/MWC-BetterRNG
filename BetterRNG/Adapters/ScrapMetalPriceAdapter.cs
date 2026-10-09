using MSCLoader;
using UnityEngine;

namespace BetterRNG
{
    internal class ScrapMetalPriceAdapter
    {
        private readonly PlayMakerFSM _logic;
        private readonly PlayMakerArrayListProxy _prices;

        public ScrapMetalPriceAdapter()
        {
            var scrapMetalPrice = GameObject.Find("Systems").transform.Find("ScrapMetalPrice");
            _logic = scrapMetalPrice.GetPlayMaker("Logic");
            _prices = scrapMetalPrice.gameObject.GetArrayListProxy("Prices");

        }

        public void SetLogicFsmActive(bool isActive)
        {
            _logic.enabled = isActive;
        }

        /// <summary>
        /// Adds a new value to the list of scrap metal prices. The list behaves like a queue, so the oldest value will be removed if the list is full.
        /// </summary>
        /// <param name="price">The new scrap metal price to add to the list.</param>
        public void PushPrice(float price)
        {
            _prices._arrayList.Insert(0, price);
            if (_prices._arrayList.Count > 9)
            {
                _prices._arrayList.RemoveAt(_prices._arrayList.Count - 1);
            }
        }
    }
}
