using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GStreamerPlayer
{
    public static class Extensions
    {
        public static TimeSpan RoundSeconds(this TimeSpan span, int nDigits)
        {
            return TimeSpan.FromSeconds(Math.Round(span.TotalSeconds, nDigits));
        }
    }
}
