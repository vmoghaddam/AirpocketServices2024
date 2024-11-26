using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiAPSB
{
    public class Helper
    {
        public static DateTime? ConvertToDate(string dt)
        {
            //comment
            if (string.IsNullOrEmpty(dt))
                return null;
            var prts = dt.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            var date = new DateTime(prts[0], prts[1], prts[2], prts[3], prts[4], 0);
            return date;
        }

        public static DateTime? ConvertToDateNoTime(string dt)
        {
            if (string.IsNullOrEmpty(dt))
                return null;
            var prts = dt.Split('-').Select(q => Convert.ToInt32(q)).ToList();
            var date = new DateTime(prts[0], prts[1], prts[2], 0, 0, 0);
            return date;
        }
    }
}