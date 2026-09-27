using System.Collections.Generic;
using MSCLoader;
using UnityEngine;

namespace BetterRNG
{
    public class BetterRNG : Mod
    {
        public override string ID => "BetterRNG"; // Your (unique) mod ID 
        public override string Name => "BetterRNG"; // Your mod name
        public override string Author => "casper-3"; // Name of the Author (your name)
        public override string Version => "0.2.0"; // Version
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
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.ModSettings, Mod_Settings);
            SetupFunction(Setup.ModSettingsLoaded, Mod_SettingsLoaded);
        }

        private void Mod_Settings()
        {
            // All settings should be created here. 
            // DO NOT put anything that isn't settings or keybinds in here!
            activeSeedText = Settings.AddText("Seed:", TextAlignment.Center);
            enableUserSeedSetting = Settings.AddCheckBox("enableUserSeed", "Use fixed seed for RNG", false, onValueChanged: OnEnableUserSeedChanged);
            userSeedSetting = Settings.AddTextBox("userSeed", "Seed", string.Empty, "If left blank a random seed will be used.", visibleByDefault: false);
        }

        private void Mod_SettingsLoaded()
        {
            if (SaveLoad.ValueExists(this, "seed"))
            {
                savedSeed = SaveLoad.ReadValue<string>(this, "seed");
            }

            GenerateRandomSeed();

            SetActiveSeedText("The seed value will appear here after loading.");
            OnEnableUserSeedChanged();
        }

        private void Mod_OnNewGame()
        {
            // Called once, when creating a new game. This is useful for deleting old mod saves
            isNewGame = true;
            GenerateRandomSeed();
        }

        private void Mod_OnLoad()
        {
            // Called once, when mod is loading after game is fully loaded
            activeSeed = SeedResolver.ResolveSeed(userSeedSetting.GetValue(), savedSeed, randomSeed, isNewGame, enableUserSeedSetting.GetValue());
            SetActiveSeedText(activeSeed);

            if (isNewGame)
            {
                SaveLoad.WriteValue(this, "seed", activeSeed);
            }

            isNewGame = false;

            var valueNoise = new ValueNoise(size: 64, seed: activeSeed.GetHashCode());

            var temperatureFn = new TemperatureGenerator([
                new NoiseFunction1D(valueNoise, 0.35f, 0.85f, 0f),
                new NoiseFunction1D(valueNoise, 0.6f, 0.15f, 0f)],
                coldNoise: new NoiseFunction1D(valueNoise, 0.2f, 1f, 0f),
                warmupPeriodDays: 7f,
                startingTemperature: -7f)
            {
                Amplitude = 15f,
                Offset = -16f,
                ColdAmplitude = 15f,
            };

            var snowGenerator = new SnowForecastGenerator(new System.Random(activeSeed.GetHashCode()));

            var weather_fix = new WeatherPatcher(temperatureFn, snowGenerator);
            weather_fix.Patch();


#if DEBUG
            // Create a graph of the temperature generator function
            int sampleCount = 30;
            List<float> xs = new(sampleCount);
            List<float> ys = new(sampleCount);
            for (int i = 0; i < sampleCount; i++)
            {
                float x = i * 1f;
                xs.Add(x);
                ys.Add(temperatureFn.Sample(x));
            }

            CreateDebugGraphWindow(xs, ys, "Temperature by day", -45f, 0f);
#endif
        }

        private void CreateDebugGraphWindow(List<float> xs, List<float> ys, string title, float minY, float maxY)
        {
            // Create a GameObject to host the graph window
            var go = new GameObject("ValueNoiseGraphWindow");
            var graph = go.AddComponent<DebugGraphWindow>();
            graph.Init(xs, ys, title, minY, maxY);
        }

        private void GenerateRandomSeed()
        {
            randomSeed = new System.Random().Next().ToString();
        }

        private void SetActiveSeedText(string value)
        {
            activeSeedText.SetValue($"Active Seed: {value}");
        }

        private void OnEnableUserSeedChanged()
        {
            userSeedSetting.SetVisibility(enableUserSeedSetting.GetValue());
        }
    }
}
