using HutongGames.PlayMaker;
using MSCCoreLibrary;

namespace BetterRNG
{
    /// <summary>
    /// Class for easy access to the some game time information.
    /// </summary>
    public class TimeAdapter
    {
        private readonly FsmInt _daysPassed;
        private readonly FsmInt _weekDay;
        public TimeAdapter()
        {
            _daysPassed = FsmVariables.GlobalVariables.GetFsmInt("GlobalDaysPassed");
            _weekDay = FsmVariables.GlobalVariables.GetFsmInt("GlobalDay");
        }

        /// <summary>
        /// Number of days passed in the save file. Starts at 0.
        /// </summary>
        /// <returns>The number of days passed.</returns>
        public int GetDaysPassed()
        {
            return _daysPassed.Value;
        }

        /// <summary>
        /// Game time weekday code normalized to 0-based index. (0 = Monday, 6 = Sunday)
        /// </summary>
        /// <returns>The 0-based index of the current weekday.</returns>
        public int GetWeekDay()
        {
            return _weekDay.Value - 1;
        }

        /// <summary>
        /// Days passed in the save file as a float, including the fraction of the current day that has passed.
        /// </summary>
        /// <returns>The number of days passed as a float, including the fraction of the current day.</returns>
        public float GetDayFraction()
        {
            return GetDaysPassed() + GameTime.Hour / 24f + GameTime.Minute / 1440f;
        }
    }
}
