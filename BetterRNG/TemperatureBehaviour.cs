using System.Collections;
using System;
using System.Collections;
using UnityEngine;

namespace BetterRNG
{
    internal class TemperatureBehaviour : MonoBehaviour
    {
        private Action _updateTemperature;

        internal void Initialize(Action updateTemperature)
        {
            _updateTemperature = updateTemperature;
        }

        private void Start()
        {
            StartCoroutine(UpdateTemperature());
        }

        private IEnumerator UpdateTemperature()
        {
            while (true)
            {
                if (_updateTemperature != null)
                {
                    _updateTemperature();
                }

                yield return new WaitForSeconds(5f);
            }
        }
    }
}
