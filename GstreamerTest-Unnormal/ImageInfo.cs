using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GstreamerTest_Unnormal
{
	public readonly struct ImageInfo : IEquatable<ImageInfo>
	{
		// Token: 0x0600018E RID: 398 RVA: 0x00011ACC File Offset: 0x0000FCCC
		public ImageInfo(int width, int height, PixelFormat format, string originalFormat = null)
		{
			if (width < 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}
			if (height < 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}
			this.Width = width;
			this.Height = height;
			this.Format = format;
			this.OriginalFormat = (originalFormat ?? format.ToString());
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00011B25 File Offset: 0x0000FD25
		public void Split(out int width, out int height, out PixelFormat format, out string originalFormat)
		{
			width = this.Width;
			height = this.Height;
			format = this.Format;
			originalFormat = this.OriginalFormat;
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000190 RID: 400 RVA: 0x00011B48 File Offset: 0x0000FD48
		public int PixelSize
		{
			get
			{
				switch (this.Format)
				{
					case PixelFormat.Unknown:
						return 0;
					case PixelFormat.R8:
						return 1;
					case PixelFormat.R16:
						return 2;
					case PixelFormat.R32F:
					case PixelFormat.R8G8B8X8:
					case PixelFormat.R8G8B8A8:
					case PixelFormat.B8G8R8X8:
					case PixelFormat.B8G8R8A8:
						return 4;
					case PixelFormat.R8G8B8:
					case PixelFormat.B8G8R8:
						return 3;
					case PixelFormat.R32G32B32A32F:
						return 16;
					case PixelFormat.R32G32F:
						return 8;
					default:
						throw new NotImplementedException();
				}
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000191 RID: 401 RVA: 0x00011BA8 File Offset: 0x0000FDA8
		public int ImageSize
		{
			get
			{
				return this.Height * this.ScanSize;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000192 RID: 402 RVA: 0x00011BB7 File Offset: 0x0000FDB7
		public int ScanSize
		{
			get
			{
				return this.Width * this.PixelSize;
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00011BC6 File Offset: 0x0000FDC6
		public bool Equals(ImageInfo other)
		{
			return this == other;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00011BD4 File Offset: 0x0000FDD4
		public override bool Equals(object obj)
		{
			return obj is ImageInfo && this.Equals((ImageInfo)obj);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00011BEC File Offset: 0x0000FDEC
		public override int GetHashCode()
		{
			return this.Width ^ this.Height ^ this.Format.GetHashCode();
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00011C0D File Offset: 0x0000FE0D
		public static bool operator ==(ImageInfo a, ImageInfo b)
		{
			return a.Width == b.Width && a.Height == b.Height && a.Format == b.Format && a.OriginalFormat == b.OriginalFormat;
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00011C4C File Offset: 0x0000FE4C
		public static bool operator !=(ImageInfo a, ImageInfo b)
		{
			return !(a == b);
		}

		// Token: 0x0400007C RID: 124
		public readonly int Width;

		// Token: 0x0400007D RID: 125
		public readonly int Height;

		// Token: 0x0400007E RID: 126
		public readonly PixelFormat Format;

		// Token: 0x0400007F RID: 127
		public readonly string OriginalFormat;
	}
}
