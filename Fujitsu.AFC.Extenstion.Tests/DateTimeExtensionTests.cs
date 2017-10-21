using System;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.AFC.Extenstion.Tests
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        private DateTime _today;

        [TestInitialize]
        public void TestInitialize()
        {
            _today = DateTime.Now;
        }

        [TestMethod]
        public void DateTimeExtensions_AddWorkingDays_Add1DayToMonday_ReturnsTuesday()
        {
            // 10/3/2014 is monday
            var actual = new DateTime(2014, 3, 10).AddWorkingDays(1);
            var expected = new DateTime(2014, 3, 11);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DateTimeExtensions_AddWorkingDays_Add1DayToFriday_ReturnsMonday()
        {
            // 7/3/2014 is friday
            var actual = new DateTime(2014, 3, 7).AddWorkingDays(1);
            var expected = new DateTime(2014, 3, 10);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void DateTimeExtensions_AddWorkingDays_Add1DayToSaturday_ReturnsMonday()
        {
            // 8/3/2014 is saturday
            var actual = new DateTime(2014, 3, 8).AddWorkingDays(1);
            var expected = new DateTime(2014, 3, 10);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DateTimeExtensions_AddWorkingDays_Add1DayToSunday_ReturnsMonday()
        {
            // 9/3/2014 is saturday
            var actual = new DateTime(2014, 3, 9).AddWorkingDays(1);
            var expected = new DateTime(2014, 3, 10);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void DateTimeExtensions_AddWorkingDays_Subtract1DayFromFriday_ReturnsThurday()
        {
            // 14/3/2014 is friday
            var actual = new DateTime(2014, 3, 14).AddWorkingDays(-1);
            var expected = new DateTime(2014, 3, 13);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void DateTimeExtensions_AddWorkingDays_Subtract3DaysFromTuesday_ReturnsThursday()
        {
            // 4/3/2014 is tuesday
            var actual = new DateTime(2014, 3, 4).AddWorkingDays(-3);
            var expected = new DateTime(2014, 2, 27);   // thursday
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void DateTimeExtensions_AddWorkingDays_AddZeroDays_DoesNothing()
        {
            var actual = new DateTime(2014, 3, 9).AddWorkingDays(0);
            var expected = new DateTime(2014, 3, 9);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void DateTimeExtensions_AddWorkingDays_Add5Days_ReturnsSameDayNextWeek()
        {
            for (int i = 0; i < 7; i++)
            {
                DateTime startDate = DateTime.Now.Date.AddDays(i);

                DateTime actual = startDate.AddWorkingDays(5);
                DateTime expected = startDate.AddDays(7);

                // Saturday or Sunday should return next Friday
                if (startDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    expected = startDate.AddDays(6);
                }

                if (startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    expected = startDate.AddDays(5);
                }

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void DateTimeExtensions_AddWorkingDays_Subtract5Days_ReturnsSameDayPreviousWeek()
        {
            for (int i = 0; i < 7; i++)
            {
                DateTime startDate = DateTime.Now.Date.AddDays(i);

                DateTime actual = startDate.AddWorkingDays(-5);
                DateTime expected = startDate.AddDays(-7);

                // Saturday or Sunday should return last Monday
                if (startDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    expected = startDate.AddDays(-5);
                }

                if (startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    expected = startDate.AddDays(-6);
                }

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void DateTimeExtensions_ToDateFirstOfMonth_DateInMiddleOfMonth_RemovesTimeElement()
        {
            var date = new DateTime(2014, 3, 12, 12, 34, 21);
            var target = date.ToFirstOfMonthDate();
            Assert.AreEqual(target.Year, 2014);
            Assert.AreEqual(target.Month, 3);
            Assert.AreEqual(target.Day, 1);
            Assert.AreEqual(target.Hour, 0);
            Assert.AreEqual(target.Minute, 0);
            Assert.AreEqual(target.Second, 0);
        }

        [TestMethod]
        public void DateTimeExtensions_IsInMonth_DateIsInMonth_ReturnsTrue()
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day);
            Assert.IsTrue(date.IsInYearMonth());
        }

        [TestMethod]
        public void DateTimeExtensions_IsInMonth_NullableDateIsInMonth_ReturnsTrue()
        {
            var now = DateTime.Now;
            var nowDate = new DateTime(now.Year, now.Month, now.Day);
            var date = new DateTime?(nowDate);
            Assert.IsTrue(date.IsInYearMonth());
        }

        [TestMethod]
        public void DateTimeExtensions_IsInMonth_DateIsNotInMonth_ReturnsTrue()
        {
            var date = new DateTime(2013, 2, 2);
            Assert.IsFalse(date.IsInYearMonth());
        }

        [TestMethod]
        public void DateTimeExtensions_IsInMonth_NullableDateIsNotInMonth_ReturnsTrue()
        {
            var nowDate = new DateTime(2013, 2, 2);
            var date = new DateTime?(nowDate);
            Assert.IsFalse(date.IsInYearMonth());
        }

        [TestMethod]
        public void DateTimeExtensions_IsInSuppliedMonth_DateIsInMonth_ReturnsTrue()
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day);
            Assert.IsTrue(date.IsInSuppliedYearMonth(now.Year, now.Month));
        }

        [TestMethod]
        public void DateTimeExtensions_IsInSuppliedMonth_NullableDateIsInMonth_ReturnsTrue()
        {
            var now = DateTime.Now;
            var nowDate = new DateTime(now.Year, now.Month, now.Day);
            var date = new DateTime?(nowDate);
            Assert.IsTrue(date.IsInSuppliedYearMonth(now.Year, now.Month));
        }

        [TestMethod]
        public void DateTimeExtensions_IsInSuppliedMonth_DateIsNotInMonth_ReturnsTrue()
        {
            var date = new DateTime(2013, 2, 2);
            Assert.IsFalse(date.IsInSuppliedYearMonth(2012, 2));
        }

        [TestMethod]
        public void DateTimeExtensions_IsInSuppliedMonth_NullableDateIsNotInMonth_ReturnsTrue()
        {
            var nowDate = new DateTime(2013, 2, 2);
            var date = new DateTime?(nowDate);
            Assert.IsFalse(date.IsInSuppliedYearMonth(2021, 12));
        }

        [TestMethod]
        public void DateTimeExtensions_IsInMonth_NullableDateSupplied_ReturnsFalse()
        {
            var date = new DateTime?();
            Assert.IsFalse(date.IsInYearMonth());
        }

        [TestMethod]
        public void DateTimeExtensions_IsInSuppliedMonth_NullableDateSupplied_ReturnsFalse()
        {
            var date = new DateTime?();
            Assert.IsFalse(date.IsInSuppliedYearMonth(2021, 12));
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsOneOff_ReturnsNull()
        {
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.OneTime);
            Assert.IsNull(nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsOneMinute_AddsOneMinuteToTaskTypeOfDay()
        {
            var expectedDate = _today.AddMinutes(1);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Minute + "1");
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsFiveMinutes_AddsFiveMinuteTsoTaskTypeOfDay()
        {
            var expectedDate = _today.AddMinutes(5);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Minute + "5");
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsFiveMinutesWithLeadingZeroes_AddsFiveMinutesToTaskTypeOfDay()
        {
            var expectedDate = _today.AddMinutes(5);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Minute + "005");
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsThirtyMinutes_AddsThirtyMinutesToTaskTypeOfDay()
        {
            var expectedDate = _today.AddMinutes(30);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Minute + "30");
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_ParseErrors_AddsTenMinutesToTaskTypeOfDay()
        {
            var expectedDate = _today.AddMinutes(10);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Minute + "Ooops");
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsOneHour_AddsOneHourToTaskTypeOfDay()
        {
            var expectedDate = _today.AddHours(1);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Hour + "1");
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsSixHours_AddsSixHoursTsoTaskTypeOfDay()
        {
            var expectedDate = _today.AddHours(6);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Hour + "6");
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsSixHoursWithLeadingZeroes_AddsSixHoursToTaskTypeOfDay()
        {
            var expectedDate = _today.AddHours(6);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Hour + "006");
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsTwelveHours_AddsThirtyMinutesToTaskTypeOfDay()
        {
            var expectedDate = _today.AddHours(12);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Hour + "12");
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_ParseErrors_AddsOneHourToTaskTypeOfDay()
        {
            var expectedDate = _today.AddHours(1);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Hour + "Ooops");
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }


        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsDaily_AddsOneDayToTaskTypeOfDay()
        {
            var expectedDate = _today.AddDays(1);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Daily);
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsWeekly_AddsOneWeekToTaskTypeOfWeek()
        {
            var expectedDate = _today.AddDays(7);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Weekly);
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsMonthly_AddsOneMonthToReportTypeOfMonth()
        {
            var expectedDate = _today.AddMonths(1);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Monthly);
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }

        [TestMethod]
        public void DateTimeExtensions_ScheduleNextRunDateTime_IsYearly_AddsOneYearToReportTypeOfYear()
        {
            var expectedDate = _today.AddYears(1);
            var nextScheduledDate = _today.ScheduleNextRunDateTime(TaskFrequencyNames.Yearly);
            Assert.AreEqual(expectedDate, nextScheduledDate);
        }
    }
}
