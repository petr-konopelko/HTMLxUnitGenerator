using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLxUnitGenerator.Extensions
{
    public static class DoubleExtension
    {
        public static string ToStringEn(this double number)
        {
            return number.ToString(new CultureInfo("en-US"));
        }

        public static string ToReportString(this double duration)
        {
            if (duration > 3600)
            {
                double hours = Math.Floor(duration / 3600);
                double minutes = Math.Floor((duration - hours * 3600) / 60);
                double seconds = Math.Round(duration - (hours * 3600 + minutes * 60));
                return String.Format("{0}:{1}:{2} h", hours, minutes, seconds.ToString("0#"));
            }
            else if (duration > 60)
            {
                double minutes = Math.Floor(duration / 60);
                double seconds = Math.Round(duration - minutes * 60);
                return String.Format("{0}:{1} m", minutes, seconds.ToString("0#"));
            }
            else
            {
                return String.Format("{0} s", Math.Round(duration, 2));
            }
        }
    }
}
