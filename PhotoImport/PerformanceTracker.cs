using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PhotoImport
{
    public class PerformanceTracker
    {
        private Stopwatch copyStopwatch;
        private Stopwatch processingStopwatch;
        public long TotalBytesCopied { get; set; }
        public int TotalFilesCopied { get; set; }
        public long CopyTimeElapsed { get; set; }
        public long ProcessingTimeElapsed { get; set; }
        public int TotalSourceDirectories { get; set; }
        public int SourceDirectoriesIgnoredByUser { get; set; }

        public TimeSpan ProcessingTime => TimeSpan.FromMilliseconds(ProcessingTimeElapsed);
        public TimeSpan CopyTime => TimeSpan.FromMilliseconds(CopyTimeElapsed);


        public PerformanceTracker()
        {
            copyStopwatch = new Stopwatch();
            processingStopwatch = new Stopwatch();
        }

        public void StartProcessingTimer()
        {
            processingStopwatch.Start();
        }

        public void StopProcessingTimer()
        {
            processingStopwatch.Stop();
            ProcessingTimeElapsed += processingStopwatch.ElapsedMilliseconds;
            processingStopwatch.Reset();
        }


        public void StartCopyTimer()
        {
            copyStopwatch.Start();
        }

        public void StopCopyTimer()
        {
            copyStopwatch.Stop();
            CopyTimeElapsed += copyStopwatch.ElapsedMilliseconds;
            copyStopwatch.Reset();
        }

        public void Report()
        {
            Console.WriteLine("Image import completed");
            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Total Processing Time: {ProcessingTime.ToUserFriendlyString()}");
            Console.WriteLine($"Total Source Directories: {TotalSourceDirectories}");
            Console.WriteLine($"Total Source Directories Ignored by User: {SourceDirectoriesIgnoredByUser}");
            Console.WriteLine($"Total Files Copied: {TotalFilesCopied}");
            Console.WriteLine($"Total Data Copied: {TotalBytesCopied.SizeSuffix()}");
            Console.WriteLine($"Total Copying Time: {CopyTime.ToUserFriendlyString()}");
        }
    }

    public static class FormatBytes
    {
        static readonly string[] SizeSuffixes =
            {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        public static string SizeSuffix(this long value)
        {
            var decimalPlaces = 1;
            if (decimalPlaces < 0)
            {
                throw new ArgumentOutOfRangeException("decimalPlaces");
            }
            if (value < 0)
            {
                return "-" + SizeSuffix(-value);
            }
            if (value == 0)
            {
                return string.Format("{0:n" + decimalPlaces + "} bytes", 0);
            }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int) Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal) value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }
    
    static class TimeSpanHelper
{
    /// <summary>
    /// Constructs a user-friendly string for this TimeSpan instance.
    /// </summary>
    public static string ToUserFriendlyString(this TimeSpan span)
    {
        const int DaysInYear = 365;
        const int DaysInMonth = 30;

        // Get each non-zero value from TimeSpan component
        List<string> values = new List<string>();

        // Number of years
        int days = span.Days;
        if (days >= DaysInYear)
        {
            int years = (days / DaysInYear);
            values.Add(CreateValueString(years, "year"));
            days = (days % DaysInYear);
        }
        // Number of months
        if (days >= DaysInMonth)
        {
            int months = (days / DaysInMonth);
            values.Add(CreateValueString(months, "month"));
            days = (days % DaysInMonth);
        }
        // Number of days
        if (days >= 1)
            values.Add(CreateValueString(days, "day"));
        // Number of hours
        if (span.Hours >= 1)
            values.Add(CreateValueString(span.Hours, "hour"));
        // Number of minutes
        if (span.Minutes >= 1)
            values.Add(CreateValueString(span.Minutes, "minute"));
        // Number of seconds (include when 0 if no other components included)
        if (span.Seconds >= 1 || values.Count == 0)
            values.Add(CreateValueString(span.Seconds, "second"));

        // Combine values into string
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < values.Count; i++)
        {
            if (builder.Length > 0)
                builder.Append((i == (values.Count - 1)) ? " and " : ", ");
            builder.Append(values[i]);
        }
        // Return result
        return builder.ToString();
    }

    /// <summary>
    /// Constructs a string description of a time-span value.
    /// </summary>
    /// <param name="value">The value of this item</param>
    /// <param name="description">The name of this item (singular form)</param>
    private static string CreateValueString(int value, string description)
    {
        return String.Format("{0:#,##0} {1}",
            value, (value == 1) ? description : String.Format("{0}s", description));
    }
}
}