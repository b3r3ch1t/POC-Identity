using System;

namespace POC.Domain.Core.Extensions
{
    public static class DateTimeExtension
    {
        public static string ToStringDataComplete(this DateTime date)
        {
            return date.ToString("G");
        }

        public static long ToUnixTimeLong(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public static string ToUnixTimeString(this DateTime date)
        {
            var result = date.ToUnixTimeLong().ToString();
            return result;
        }
    }
}
