using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace HTMLxUnitGenerator
{
    public static class ConvertionHelpers
    {
        private static IFormatProvider provider = new CultureInfo("en-US");

        public static double ToDouble(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return 0;
            else
                return Double.Parse(text.Replace(',', '.'), provider);
        }

        public static DateTime ToDateTime(this string text) => DateTime.Parse(text);
    }
}
