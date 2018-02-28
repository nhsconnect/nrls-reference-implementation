using System;
using System.Text.RegularExpressions;

namespace Demonstrator.Utilities.Extensions
{
    public static class DateTimeExt
    {
        public static DateTimeOffset? ToDateTimeOffset(this DateTime? datetime)
        {
            if(!datetime.HasValue)
            {
                return datetime;
            }

            DateTimeOffset? dtOffset = datetime.Value;

            return dtOffset;
        }

        public static DateTime? ToDateTime(this string date)
        {

            var regex = new Regex("-?([0-9]{4})(-(0[1-9]|1[0-2])(-(0[0-9]|[1-2][0-9]|3[0-1]))?)?");

            var dateMatch = regex.Match(date);
            var groupCount = dateMatch.Groups.Count;

            if (!dateMatch.Success)
            {
                return null;
            }

            int year = int.Parse(dateMatch.Groups[1].Value); //There is always a year
            int month = 01;
            int day = 01;

            if(dateMatch.Groups[3].Success)
            {
                month = int.Parse(dateMatch.Groups[3].Value);
            }

            if (dateMatch.Groups[5].Success)
            {
                day = int.Parse(dateMatch.Groups[5].Value);
            }

            return new DateTime(year, month, day);

        }
    }
}
