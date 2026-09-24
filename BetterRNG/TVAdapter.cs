using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace BetterRNG
{
    internal class TVAdapter
    {
        private readonly FsmFloat _noiseAlpha;
        private readonly AudioSource _tvNoiseAudio;

        public TVAdapter()
        {
            var tv = GameObject.Find("Systems").transform.Find("TV");
            _noiseAlpha = tv.Find("TVNoise").GetPlayMaker("Animate").FsmVariables.GetFsmFloat("Alpha");
            _tvNoiseAudio = tv.Find("TVPrograms/TVprogramFlicker/TVAudioNoise").GetComponent<AudioSource>();
        }

        public void SetAlpha(float value)
        {
            _noiseAlpha.Value = value;
        }

        public void SetAudioVolume(float value)
        {
            _tvNoiseAudio.volume = value;
        }

        public void ResetTvNoise()
        {
            SetAlpha(1.1f);
            SetAudioVolume(0f);
        }
    }
}
