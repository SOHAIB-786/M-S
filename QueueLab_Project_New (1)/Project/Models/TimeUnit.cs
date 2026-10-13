using System;

namespace QueueSimulatorApp.Models
{
    /// <summary>
    /// Time units offered in every drop-down (inputs and output).
    /// The ORDER matters: it must match the ComboBox item order in the XAML
    /// (0 = Seconds, 1 = Minutes, 2 = Hours).
    /// </summary>
    public enum TimeUnit
    {
        Seconds = 0,
        Minutes = 1,
        Hours = 2
    }

    public static class TimeUnitInfo
    {
        /// <summary>How many MINUTES are in one unit (the calculator works internally in minutes).</summary>
        public static double MinutesPerUnit(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds: return 1.0 / 60.0;
                case TimeUnit.Hours: return 60.0;
                default: return 1.0;
            }
        }

        public static TimeUnit FromIndex(int index)
        {
            if (index < 0) index = 1;
            if (index > 2) index = 2;
            return (TimeUnit)index;
        }

        public static string Short(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds: return "sec";
                case TimeUnit.Hours: return "hr";
                default: return "min";
            }
        }

        public static string Singular(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds: return "second";
                case TimeUnit.Hours: return "hour";
                default: return "minute";
            }
        }

        public static string Plural(TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Seconds: return "seconds";
                case TimeUnit.Hours: return "hours";
                default: return "minutes";
            }
        }
    }
}
