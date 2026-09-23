using System;
using System.Collections.Generic;
using MSCLoader;

namespace BetterRNG
{
    public class BetterRNG : Mod
    {
        public override string ID => "BetterRNG"; // Your (unique) mod ID 
        public override string Name => "BetterRNG"; // Your mod name
        public override string Author => "casper-3"; // Name of the Author (your name)
        public override string Version => "0.1.0"; // Version
        public override string Description => "Replaces random walk method used in weather generation and other systems."; // Short description of your mod 
        public override Game SupportedGames => Game.MyWinterCar;


        private bool isNewGame = false;
        private string activeSeed = null;
        private string savedSeed;
        private string randomSeed;
        private SettingsText activeSeedText;
        private SettingsCheckBox enableUserSeedSetting;
        private SettingsTextBox userSeedSetting;


        public override void ModSetup()
        {
            SetupFunction(Setup.OnNewGame, Mod_OnNewGame);
            SetupFunction(Setup.PreLoad, Mod_OnPreload);
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.ModSettings, Mod_Settings);
            SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoaded);
        }

        private void Mod_Settings()
        {
            // All settings should be created here. 
            // DO NOT put anything that isn't settings or keybinds in here!
            activeSeedText = Settings.AddText("Seed:");
            enableUserSeedSetting = Settings.AddCheckBox("enableUserSeed", "Use fixed seed for RNG", false, onValueChanged: OnEnableUserSeedChanged);
            userSeedSetting = Settings.AddTextBox("userSeed", "Seed", string.Empty, "If left blank a random seed will be used.", visibleByDefault: false);
        }

        private void Mod_SettingsLoaded()
        {
            if (SaveLoad.ValueExists(this, "seed"))
            {
                savedSeed = SaveLoad.ReadValue<string>(this, "seed");
            }
            else
            {
                GenerateRandomSeed();
            }

            OnEnableUserSeedChanged();
        }

        private void Mod_OnNewGame()
        {
            // Called once, when creating a new game. This is useful for deleting old mod saves
            isNewGame = true;
            GenerateRandomSeed();
        }
        private void Mod_OnPreload()
        {

            activeSeed = SeedResolver.ResolveSeed(userSeedSetting.GetValue(), savedSeed, randomSeed, isNewGame, enableUserSeedSetting.GetValue());
            SetActiveSeedText(activeSeed);

            if (isNewGame)
            {
                SaveLoad.WriteValue(this, "seed", activeSeed);
            }

            isNewGame = false;
        }

        private void Mod_OnLoad()
        {
            // Called once, when mod is loading after game is fully loaded
            var weather_fn = new TemperatureGenerator(activeSeed.GetHashCode());

            var weather_fix = new WeatherPatcher(weather_fn);
            weather_fix.Patch();


            // DEBUG: Create a graph of the temperature generator function
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

        private void GenerateRandomSeed()
        {
            randomSeed = new Random().Next().ToString();
        }

        private void SetActiveSeedText(string value)
        {
            activeSeedText.SetValue($"Seed: {value}");
        }

        private void OnEnableUserSeedChanged()
        {
            userSeedSetting.SetVisibility(enableUserSeedSetting.GetValue());
        }
    }
}
