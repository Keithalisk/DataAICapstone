using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace DataAICapstone.Plugins
{
    /// <summary>
    /// Time and date plugin for Semantic Kernel agents
    /// </summary>
    public class TimePlugin
    {
        [KernelFunction]
        [Description("Get the current date and time")]
        public string GetCurrentDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        [KernelFunction]
        [Description("Get the current date")]
        public string GetCurrentDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        [KernelFunction]
        [Description("Get the current time")]
        public string GetCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        [KernelFunction]
        [Description("Get the current UTC date and time")]
        public string GetCurrentUtcDateTime()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");
        }

        [KernelFunction]
        [Description("Add days to a date")]
        public string AddDaysToDate(
            [Description("The date in yyyy-MM-dd format")] string date,
            [Description("Number of days to add (can be negative)")] int days)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                return parsedDate.AddDays(days).ToString("yyyy-MM-dd");
            }
            throw new ArgumentException("Invalid date format. Use yyyy-MM-dd");
        }

        [KernelFunction]
        [Description("Calculate the difference between two dates in days")]
        public int GetDaysBetweenDates(
            [Description("The first date in yyyy-MM-dd format")] string date1,
            [Description("The second date in yyyy-MM-dd format")] string date2)
        {
            if (DateTime.TryParse(date1, out DateTime parsedDate1) && 
                DateTime.TryParse(date2, out DateTime parsedDate2))
            {
                return (int)(parsedDate2 - parsedDate1).TotalDays;
            }
            throw new ArgumentException("Invalid date format. Use yyyy-MM-dd");
        }

        [KernelFunction]
        [Description("Get the day of the week for a date")]
        public string GetDayOfWeek([Description("The date in yyyy-MM-dd format")] string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                return parsedDate.DayOfWeek.ToString();
            }
            throw new ArgumentException("Invalid date format. Use yyyy-MM-dd");
        }

        [KernelFunction]
        [Description("Format a date and time")]
        public string FormatDateTime(
            [Description("The date and time to format")] string dateTime,
            [Description("The format string (e.g., 'MMM dd, yyyy')")] string format)
        {
            if (DateTime.TryParse(dateTime, out DateTime parsedDateTime))
            {
                return parsedDateTime.ToString(format);
            }
            throw new ArgumentException("Invalid date time format");
        }
    }
}
