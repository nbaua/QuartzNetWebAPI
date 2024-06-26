﻿using QuartzNetWebAPI.Models.Triggers.SchedulerDescriptors;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace QuartzNetWebAPI.Models.Triggers
{
    /// <summary>
    /// Extension methods Trigger details.
    /// </summary>
    public static class TriggerExtensions
    {
        /// <summary>
        /// Extension for getting trigger type from a <see cref="ITrigger"/> instance. 
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static TriggerType GetTriggerType(this ITrigger trigger)
        {
            switch(trigger)
            {
                case ICronTrigger _:
                    return TriggerType.Cron;
                case IDailyTimeIntervalTrigger _:
                    return TriggerType.Daily;
                case ISimpleTrigger _:
                    return TriggerType.Simple;
                case ICalendarIntervalTrigger _:
                    return TriggerType.Calendar;
                default:
                    return TriggerType.Unknown;
            }
        }

        /// <summary>
        /// Extension method for getting scheduling description for a <see cref="ITrigger"/>
        /// </summary>
        /// <param name="trigger">The <see cref="ITrigger"/></param>
        /// <returns>String containing the scheduling description for the <see cref="ITrigger"/></returns>
        public static string GetScheduleDescription(this ITrigger trigger)
        {
            switch(trigger)
            {
                case ICronTrigger cr:
                    return ExpressionDescriptor.GetDescription(cr.CronExpressionString);
                case IDailyTimeIntervalTrigger dt:
                    return GetScheduleDescription(dt);
                case ISimpleTrigger st:
                    return GetScheduleDescription(st);
                case ICalendarIntervalTrigger ct:
                    return GetScheduleDescription(ct.RepeatInterval, ct.RepeatIntervalUnit);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Extension method for getting scheduling description for a <see cref="ITrigger"/>
        /// </summary>
        /// <param name="trigger">The <see cref="IDailyTimeIntervalTrigger"/></param>
        /// <returns>String containing the scheduling description for the <see cref="ITrigger"/></returns>
        public static string GetScheduleDescription(this IDailyTimeIntervalTrigger trigger)
        {
            var result = GetScheduleDescription(trigger.RepeatInterval, trigger.RepeatIntervalUnit, trigger.RepeatCount);
            result += " from " + trigger.StartTimeOfDay.ToShortFormat() + " to " + trigger.EndTimeOfDay.ToShortFormat();

            if(trigger.DaysOfWeek.Count < 7)
            {
                var dow = DaysOfWeekViewModel.Create(trigger.DaysOfWeek);

                if(dow.AreOnlyWeekdaysEnabled)
                    result += " only on Weekdays";
                else if(dow.AreOnlyWeekendEnabled)
                    result += " only on Weekends";
                else
                    result += " on " + string.Join(", ", trigger.DaysOfWeek);
            }

            return result;
        }

        /// <summary>
        /// Extension method for getting scheduling description for a <see cref="ITrigger"/>
        /// </summary>
        /// <param name="trigger">The <see cref="ITrigger"/></param>
        /// <returns>String containing the scheduling description for the <see cref="ITrigger"/></returns>
        public static string GetScheduleDescription(this ISimpleTrigger trigger)
        {
            var result = "Repeat ";
            if(trigger.RepeatCount > 0)
                result += trigger.RepeatCount + " times ";
            result += "every ";

            var diff = trigger.RepeatInterval.TotalMilliseconds;

            var messagesParts = new List<string>();
            foreach(var part in TimespanPart.Items)
            {
                var currentPartValue = Math.Floor(diff / part.Multiplier);
                diff -= currentPartValue * part.Multiplier;

                if(currentPartValue == 1)
                    messagesParts.Add(part.Singular);
                else if(currentPartValue > 1)
                    messagesParts.Add(currentPartValue + " " + part.Plural);
            }

            result += string.Join(", ", messagesParts);

            return result;
        }

        /// <summary>
        /// Helper method for getting scheduling description for a <see cref="ITrigger"/>
        /// </summary>
        /// <param name="repeatInterval">Time to wait between each time a trigger fires.</param>
        /// <param name="repeatIntervalUnit">Time interval to use for the repeatInterval.</param>
        /// <param name="repeatCount">How many times a trigger will be fired.</param>
        /// <returns>String containing the scheduling description for the <see cref="ITrigger"/></returns>
        public static string GetScheduleDescription(int repeatInterval, IntervalUnit repeatIntervalUnit, int repeatCount = 0)
        {
            var result = "Repeat ";
            if(repeatCount > 0)
                result += repeatCount + " times ";
            result += "every ";

            var unitStr = repeatIntervalUnit.ToString().ToLower();

            if(repeatInterval == 1)
                result += unitStr;
            else
                result += repeatInterval + " " + unitStr + "s";

            return result;
        }




        private class TimespanPart
        {
            public static readonly TimespanPart[] Items = new[]
            {
            new TimespanPart("day", 1000 * 60 * 60 * 24),
            new TimespanPart("hour", 1000 * 60 * 60),
            new TimespanPart("minute", 1000 * 60),
            new TimespanPart("second", 1000),
            new TimespanPart("millisecond", 1),
        };

            public string Singular { get; set; }
            public string Plural { get; set; }
            public long Multiplier { get; set; }

            public TimespanPart(string singular, long multiplier) : this(singular)
            {
                Multiplier = multiplier;
            }
            public TimespanPart(string singular)
            {
                Singular = singular;
                Plural = singular + "s";
            }
        }

        /// <summary>
        /// Extension method for converting a <see cref="TimeOfDay"/> object to a readable string.
        /// </summary>
        /// <param name="timeOfDay">The object to convert.</param>
        /// <returns>A readable time of day string in short format.</returns>
        public static string ToShortFormat(this TimeOfDay timeOfDay)
        {
            return timeOfDay.ToTimeSpan().ToString("g", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Extension method for converting a <see cref="TimeOfDay"/> object to <see cref="TimeSpan"/> object.
        /// </summary>
        /// <param name="timeOfDay">The object to convert from.</param>
        /// <returns>The converted object.</returns>
        public static TimeSpan ToTimeSpan(this TimeOfDay timeOfDay)
        {
            return TimeSpan.FromSeconds(timeOfDay.Second + timeOfDay.Minute * 60 + timeOfDay.Hour * 3600);
        }

        /// <summary>
        /// Data model for describing days in a week.
        /// </summary>
        public class DaysOfWeekViewModel
        {
            public bool Monday { get; set; }
            public bool Tuesday { get; set; }
            public bool Wednesday { get; set; }
            public bool Thursday { get; set; }
            public bool Friday { get; set; }
            public bool Saturday { get; set; }
            public bool Sunday { get; set; }

            /// <summary>
            /// Setting all days to true.
            /// </summary>
            public void AllOn()
            {
                Monday = true;
                Tuesday = true;
                Wednesday = true;
                Thursday = true;
                Friday = true;
                Saturday = true;
                Sunday = true;
            }

            /// <summary>
            /// Populating the data model.
            /// </summary>
            /// <param name="list">List of <see cref="DayOfWeek"/> to populate.</param>
            /// <returns>The populated <see cref="DaysOfWeekViewModel"/></returns>
            public static DaysOfWeekViewModel Create(IEnumerable<DayOfWeek> list)
            {
                var model = new DaysOfWeekViewModel();
                foreach(var item in list)
                {
                    switch(item)
                    {
                        case DayOfWeek.Sunday:
                            model.Sunday = true;
                            break;
                        case DayOfWeek.Monday:
                            model.Monday = true;
                            break;
                        case DayOfWeek.Tuesday:
                            model.Tuesday = true;
                            break;
                        case DayOfWeek.Wednesday:
                            model.Wednesday = true;
                            break;
                        case DayOfWeek.Thursday:
                            model.Thursday = true;
                            break;
                        case DayOfWeek.Friday:
                            model.Friday = true;
                            break;
                        case DayOfWeek.Saturday:
                            model.Saturday = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                return model;
            }

            /// <summary>
            /// Getting a list of selected days from the model.
            /// </summary>
            /// <returns>The list of selected days in the model.</returns>
            public IEnumerable<DayOfWeek> GetSelected()
            {
                if(Monday) yield return DayOfWeek.Monday;
                if(Tuesday) yield return DayOfWeek.Tuesday;
                if(Wednesday) yield return DayOfWeek.Wednesday;
                if(Thursday) yield return DayOfWeek.Thursday;
                if(Friday) yield return DayOfWeek.Friday;
                if(Saturday) yield return DayOfWeek.Saturday;
                if(Sunday) yield return DayOfWeek.Sunday;
            }

            /// <summary>
            /// True if only Saturday and Sunday is true.
            /// </summary>
            public bool AreOnlyWeekendEnabled => !Monday && !Tuesday && !Wednesday && !Thursday && !Friday && Saturday && Sunday;
            /// <summary>
            /// True if only weekdays are set to true.
            /// </summary>
            public bool AreOnlyWeekdaysEnabled => Monday && Tuesday && Wednesday && Thursday && Friday && !Saturday && !Sunday;
        }

    }
}
