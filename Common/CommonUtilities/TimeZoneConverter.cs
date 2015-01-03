/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtilities
{
    /// <summary>
    /// Singleton that converts dates to different time zones.
    /// All supported time zones must exist in the registry of the host machine;
    /// otherwise, this class will throw an exception when it's first accessed.
    /// The registry key is:
    ///     HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones
    /// </summary>
    public class TimeZoneConverter
    {
        public static class TimeZoneId
        {
            public const string Pacific = "Pacific Standard Time";
            public const string Hawaiian = "Hawaiian Standard Time";
            public const string Central = "Central Standard Time";
            public const string Eastern = "Eastern Standard Time";
            public const string Mountain = "Mountain Standard Time";
        }

        public enum TimeZone
        {
            Pacific,
            Hawaiian,
            Central,
            Eastern,
            Mountain
        }

        private Dictionary<string, TimeZoneInfo> loadedTimeZones = new Dictionary<string, TimeZoneInfo>();

        private static TimeZoneConverter instance;
        static TimeZoneConverter()
        {
            instance = new TimeZoneConverter();
        }

        private TimeZoneConverter()
        {
            loadedTimeZones = new Dictionary<string, TimeZoneInfo>();
            loadedTimeZones.Add(TimeZoneId.Pacific, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId.Pacific));
            loadedTimeZones.Add(TimeZoneId.Hawaiian, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId.Hawaiian));
            loadedTimeZones.Add(TimeZoneId.Central, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId.Central));
            loadedTimeZones.Add(TimeZoneId.Eastern, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId.Eastern));
            loadedTimeZones.Add(TimeZoneId.Mountain, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId.Mountain));
        }

        public static DateTime Convert(DateTime dateToConvert, TimeZone timezone)
        {
            string tzid = TimeZoneId.Eastern; // default
            switch (timezone)
            {
                case TimeZone.Central:
                    tzid = TimeZoneId.Central;
                    break;
                case TimeZone.Hawaiian:
                    tzid = TimeZoneId.Hawaiian;
                    break;
                case TimeZone.Pacific:
                    tzid = TimeZoneId.Pacific;
                    break;
                case TimeZone.Mountain:
                    tzid = TimeZoneId.Mountain;
                    break;
            }
            return Convert(dateToConvert, tzid);
        }

        public static DateTime Convert(DateTime dateToConvert, string timeZoneId)
        {
            // if it's not a pre-loaded time zone, try to load it and add it to the collection.
            //  Note that trying to load a time zone that's not set up in the registry of the host
            //  machine will throw an exception.
            if (!instance.loadedTimeZones.ContainsKey(timeZoneId))
                instance.loadedTimeZones.Add(timeZoneId, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
            return TimeZoneInfo.ConvertTime(dateToConvert, instance.loadedTimeZones[timeZoneId]);
        }

        /// <summary>
        /// Pass the enum value as a string.  Useful when configuring the timezone in a config file.
        /// </summary>
        /// <param name="tzString">A string representation of the TimeZoneConverter.TimeZone enum</param>
        /// <returns></returns>
        public static TimeZone TimeZoneFromString(string tzString)
        {
            TimeZone tz;
            if (!Enum.TryParse<TimeZoneConverter.TimeZone>(tzString, out tz))
                tz = TimeZone.Eastern; // default to Eastern.
            return tz;
        }
    
    }
}
