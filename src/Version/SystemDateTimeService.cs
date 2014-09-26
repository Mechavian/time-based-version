using System;

namespace Version
{
    internal class SystemDateTimeService : IDateTimeService
    {
        public DateTime GetDateTimeEst()
        {
            // TODO: Consider making the time zone adjustable via command line
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time");
        }
    }
}