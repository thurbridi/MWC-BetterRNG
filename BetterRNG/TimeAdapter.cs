using HutongGames.PlayMaker;

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
            _daysPassed = FsmVariables.GlobalVariables.GetFsmInt("DaysPassed");
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
    }
}
