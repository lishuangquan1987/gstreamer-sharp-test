using System;
using System.Collections.Generic;
using System.Drawing;
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
        public static System.Drawing.Bitmap ToSize(this System.Drawing.Bitmap bitmap, int width, int height)
        {
            

            Bitmap result = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(result);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            g.DrawImage(bitmap,0,0,width,height);
            g.Dispose();
            return result;
        }
    }
}
