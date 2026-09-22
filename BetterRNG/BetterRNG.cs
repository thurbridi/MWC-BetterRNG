using System.Collections.Generic;
using MSCLoader;

namespace BetterRNG
{
    public class BetterRNG : Mod
    {
        public override string ID => "BetterRNG"; // Your (unique) mod ID 
        public override string Name => "BetterRNG"; // Your mod name
        public override string Author => "casper-3"; // Name of the Author (your name)
        public override string Version => "0.0.1"; // Version
        public override string Description => "Replaces random walk method used in weather generation and other systems."; // Short description of your mod 
        public override Game SupportedGames => Game.MyWinterCar;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnNewGame, Mod_OnNewGame);
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.OnSave, Mod_OnSave);
            SetupFunction(Setup.ModSettings, Mod_Settings);
        }

        private void Mod_Settings()
        {
            // All settings should be created here. 
            // DO NOT put anything that isn't settings or keybinds in here!
        }

        private void Mod_OnNewGame()
        {
            // Called once, when creating a new game. This is useful for deleting old mod saves
            // TODO: Renew seeds for each ValueNoise instance.
        }
        private void Mod_OnLoad()
        {
            // Called once, when mod is loading after game is fully loaded
            var weather_fix = new WeatherPatcher();
            weather_fix.Patch();

            var weather_fn = new TemperatureGenerator();

            int sampleCount = 90;
            List<float> xs = new List<float>(sampleCount);
            List<float> ys = new List<float>(sampleCount);
            for (int i = 0; i < sampleCount; i++)
            {
                float x = i * 1f;
                xs.Add(x);
                ys.Add(weather_fn.Sample(x));
            }

            // Create a GameObject to host the graph window
            var go = new UnityEngine.GameObject("ValueNoiseGraphWindow");
            var graph = go.AddComponent<DebugGraphWindow>();
            graph.Init(xs, ys, "Temperature by day");
        }

        private void Mod_OnSave()
        {

        }
    }
}
