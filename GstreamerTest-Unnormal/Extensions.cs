using Gst;
using GstreamerTest_Unnormal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GstreamerTest.Unnormal
{

   public static class Extensions
    {
        public static void SetStateBlocking(this Element element, State state)
        {
            var stateReturn = element.SetState(state);
            switch (stateReturn)
            {
                case StateChangeReturn.Failure:
                    throw new Exception();
                case StateChangeReturn.Success:
                    return;
                case StateChangeReturn.Async:
                    while (true)
                    {
                        switch (element.GetState(out var currentState, out var pendingState, Constants.CLOCK_TIME_NONE))
                        {
                            case StateChangeReturn.Failure:
                                throw new Exception();
                            case StateChangeReturn.Success:
                                return;
                            case StateChangeReturn.Async:
                                continue;
                            case StateChangeReturn.NoPreroll:
                                return;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                case StateChangeReturn.NoPreroll:
                    return;
                default:
                    throw new NotImplementedException();
            }
        }
        // Token: 0x0600018C RID: 396 RVA: 0x00011A3C File Offset: 0x0000FC3C
        public static System.Drawing.Imaging.PixelFormat ToPixelFormat(this PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.R8G8B8:
                case PixelFormat.B8G8R8:
                    return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                case PixelFormat.R8G8B8X8:
                case PixelFormat.B8G8R8X8:
                    return System.Drawing.Imaging.PixelFormat.Format32bppRgb;
                case PixelFormat.R8G8B8A8:
                case PixelFormat.B8G8R8A8:
                    return System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }
            return System.Drawing.Imaging.PixelFormat.Undefined;
        }

        // Token: 0x0400007A RID: 122
        public static readonly IImage Default = new ArrayImage<byte>(byte.MaxValue.YieldReturnMany(262144).ToArray<byte>(), new ImageInfo(256, 256, PixelFormat.R8G8B8A8, null), false);
        // Token: 0x06000180 RID: 384 RVA: 0x00011660 File Offset: 0x0000F860
        public static BitmapImage ToImage(this System.Drawing.Bitmap bitmap, bool takeOwnership, bool isVolatile = true, bool canWrite = false)
        {
            return new BitmapImage(bitmap, takeOwnership, isVolatile, canWrite);
        }
        // Token: 0x0600007D RID: 125 RVA: 0x0000E61E File Offset: 0x0000C81E
        public static IEnumerable<T> YieldReturnMany<T>(this T item, int count)
        {
            int num;
            for (int i = 0; i < count; i = num + 1)
            {
                yield return item;
                num = i;
            }
            yield break;
        }

        // Token: 0x06000189 RID: 393 RVA: 0x00011880 File Offset: 0x0000FA80
        public static void CopyTo(IImageData srcData, IImageData dstData)
        {
            ReadOnlySpan<byte> span = srcData.Bytes.Span;
            Span<byte> span2 = MemoryMarshal.AsMemory<byte>(dstData.Bytes).Span;
            int scanSize = srcData.ScanSize;
            int scanSize2 = dstData.ScanSize;
            if (scanSize == scanSize2)
            {
                span.Slice(0, span2.Length).CopyTo(span2);
                return;
            }
            int num = span.Length / scanSize;
            int length = Math.Min(scanSize, scanSize2);
            for (int i = 0; i < num; i++)
            {
                span.Slice(i * scanSize, length).CopyTo(span2.Slice(i * scanSize2, length));
            }
        }
        public static System.Drawing.Bitmap FromImage(this IImage image, bool copy)
        {
            ImageInfo info = image.Info;
            System.Drawing.Imaging.PixelFormat format = info.Format.ToPixelFormat();

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(info.Width, info.Height, format);
            BitmapImage bitmapImage = bitmap.ToImage(false, true, true);
            using (IImageData data = image.GetData())
            {
                using (IImageData data2 = bitmapImage.GetData())
                {
                    Extensions.CopyTo(data, data2);
                }
            }
            
            return bitmap;

        }

        // Token: 0x0600018B RID: 395 RVA: 0x00011A18 File Offset: 0x0000FC18
        public static PixelFormat ToPixelFormat(this System.Drawing.Imaging.PixelFormat format)
        {
            if (format == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                return PixelFormat.R8G8B8;
            }
            if (format == System.Drawing.Imaging.PixelFormat.Format32bppRgb)
            {
                return PixelFormat.B8G8R8X8;
            }
            if (format != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                return PixelFormat.Unknown;
            }
            return PixelFormat.B8G8R8A8;
        }
    }
}
