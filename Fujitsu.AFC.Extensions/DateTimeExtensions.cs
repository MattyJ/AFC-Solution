using System;
using System.Globalization;
using Fujitsu.AFC.Constants;

namespace Fujitsu.AFC.Extensions
{
    public static class DateTimeExtensions
    {
        private const string DateFormat = "dd/MM/yyyy";

        public static DateTime ToFirstOfMonthDate(this DateTime item)
        {
            return new DateTime(item.Year, item.Month, 1);
        }

        public static bool IsInYearMonth(this DateTime item)
        {
            var now = DateTime.Now;
            return now.Year == item.Year && now.Month == item.Month;
        }

        public static bool IsInSuppliedYearMonth(this DateTime item, int year, int month)
        {
            return year == item.Year && month == item.Month;
        }

        public static bool IsInYearMonth(this DateTime? item)
        {
            return item.HasValue && item.Value.IsInYearMonth();
        }

        public static bool IsInSuppliedYearMonth(this DateTime? item, int year, int month)
        {
            return item.HasValue && item.Value.IsInSuppliedYearMonth(year, month);
        }

        public static string ToDateString(this DateTime? item)
        {
            return item?.Date.ToString(DateFormat, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        public static string ToDateTimeString(this DateTime? item)
        {
            return item?.Date.ToString(DateFormat, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        public static DateTime AddWorkingDays(this DateTime date, int daysToAdd)
        {
            DayOfWeek dayOfWeek;

            while (daysToAdd > 0)
            {
                date = date.AddDays(1);

                dayOfWeek = date.DayOfWeek;
                if ((dayOfWeek != DayOfWeek.Saturday) && (dayOfWeek != DayOfWeek.Sunday))
                {
                    daysToAdd--;
                }
            }

            while (daysToAdd < 0)
            {
                date = date.AddDays(-1);

                dayOfWeek = date.DayOfWeek;
                if ((dayOfWeek != DayOfWeek.Saturday) && (dayOfWeek != DayOfWeek.Sunday))
                {
                    daysToAdd++;
                }
            }

            return date;
        }

        public static DateTime? ScheduleNextRunDateTime(this DateTime value, string frequency)
        {
            if (string.IsNullOrEmpty(frequency))
            {
                return value.AddDays(1);
            }

            bool valid;
            double duration;

            if (frequency.SafeEquals(TaskFrequencyNames.Monthly))
            {
                return value.AddMonths(1);
            }

            switch (frequency.Substring(0, 1).ToUpper())
            {
                case TaskFrequencyNames.OneTime:
                    return null;
                case TaskFrequencyNames.Minute:
                    valid = double.TryParse(frequency.Substring(1, frequency.Length - 1), out duration);
                    return value.AddMinutes(valid ? duration : 10);
                case TaskFrequencyNames.Hour:
                    valid = double.TryParse(frequency.Substring(1, frequency.Length - 1), out duration);
                    return value.AddHours(valid ? duration : 1);
                case TaskFrequencyNames.Daily:
                    return value.AddDays(1);
                case TaskFrequencyNames.Weekly:
                    return value.AddDays(7);
                case TaskFrequencyNames.Yearly:
                    return value.AddYears(1);
                default:
                    return value.AddDays(1);
            }
        }

        public static int ConvertDateToMonthNumber(this DateTime date, int startMonthNumber)
        {
            var monthNumber = ((date.Year * 12) + date.Month - startMonthNumber);
            if ((monthNumber < 1) || (monthNumber > 999))
            {
                throw new ArgumentException("Error in DateTimeExtensions:ConvertDateToMonthNumber : date '" + date.Date.ToShortDateString() + "' is out of range.");
            }
            return monthNumber;
        }
    }
}
