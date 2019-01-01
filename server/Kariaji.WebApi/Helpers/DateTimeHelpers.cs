using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kariaji.WebApi.Helpers
{
    public static class DateTimeHelpers
    {
        public static string ToHungarianDate(this DateTime dt)
        {
            return dt.ToString("yyyy.MM.dd.");
        }

        public static string ToHungarianDateTime(this DateTime dt)
        {
            return dt.ToString("yyyy.MM.dd., hh:mm");
        }
    }
}
