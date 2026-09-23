using System;

namespace BetterRNG
{
    internal static class SeedResolver
    {
        /// <summary>
        /// Resolves the appropriate seed based on the provided parameters and game state.
        /// </summary>
        /// <param name="userSeed">On new game, takes precedence if `isUserSeedPreffered == true` and is valid.</param>
        /// <param name="savedSeed">Takes precedence if valid and not a new game.</param>
        /// <param name="fallbackSeed">On new game, takes precedence if `isUserSeedPreffered == false`. Fallback seed if no other seeds are valid.</param>
        /// <param name="isNewGame">Indicates wether a new save is loading.</param>
        /// <param name="isUserSeedPreffered">Indicates whether the user's seed should be preferred on a new game.</param>
        /// <returns>One of the provided seeds based on settings and gamestate.</returns>
        public static string ResolveSeed(string userSeed, string savedSeed, string fallbackSeed, bool isNewGame, bool isUserSeedPreffered)
        {
            if (!IsValidSeed(fallbackSeed)) throw new ArgumentException("Random seed must be valid.", nameof(fallbackSeed));

            if (!isNewGame && IsValidSeed(savedSeed)) return savedSeed;

            if (isUserSeedPreffered && IsValidSeed(userSeed)) return userSeed;

            return fallbackSeed;
        }

        private static bool IsValidSeed(string seed)
        {
            return !string.IsNullOrEmpty(seed);
        }
    }
}
