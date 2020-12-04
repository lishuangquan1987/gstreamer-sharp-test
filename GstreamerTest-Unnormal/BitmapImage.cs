using GstreamerTest.Unnormal;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GstreamerTest_Unnormal
{
	// Token: 0x0200002C RID: 44
	public class BitmapImage : IImage, IDisposable
	{
		// Token: 0x0600019E RID: 414 RVA: 0x00011CDA File Offset: 0x0000FEDA
		internal BitmapImage(System.Drawing.Bitmap bitmap, bool isOwner, bool isVolatile, bool canWrite)
		{
			this.FBitmap = bitmap;
			this.FIsOwner = isOwner;
			this.IsVolatile = isVolatile;
			this.FCanWrite = canWrite;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00011D00 File Offset: 0x0000FF00
		~BitmapImage()
		{
			this.Dispose();
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x00011D2C File Offset: 0x0000FF2C
		public ImageInfo Info
		{
			get
			{
				if (this.FIsDisposed)
				{
					return GstreamerTest.Unnormal.Extensions.Default.Info;
				}
				return new ImageInfo(this.FBitmap.Width, this.FBitmap.Height, this.FBitmap.PixelFormat.ToPixelFormat(), this.FBitmap.PixelFormat.ToString());
			}
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00011D90 File Offset: 0x0000FF90
		public IImageData GetData()
		{
			if (this.FIsDisposed)
			{
				 return GstreamerTest.Unnormal.Extensions.Default.GetData();
			}
			return new BitmapImage.Data(this.FBitmap, this.FCanWrite);
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x00011DC3 File Offset: 0x0000FFC3
		public bool IsVolatile { get; }

		// Token: 0x060001A3 RID: 419 RVA: 0x00011DCB File Offset: 0x0000FFCB
		public void Dispose()
		{
			if (!this.FIsDisposed)
			{
				this.FIsDisposed = true;
				GC.SuppressFinalize(this);
				if (this.FIsOwner)
				{
					this.FBitmap.Dispose();
				}
			}
		}

		// Token: 0x04000085 RID: 133
		private readonly System.Drawing.Bitmap FBitmap;

		// Token: 0x04000086 RID: 134
		private readonly bool FIsOwner;

		// Token: 0x04000087 RID: 135
		private readonly bool FCanWrite;

		// Token: 0x04000088 RID: 136
		private bool FIsDisposed;

		// Token: 0x02000114 RID: 276
		private class Data : MemoryManager<byte>, IImageData, IDisposable
		{
			// Token: 0x0600062C RID: 1580 RVA: 0x0001C3EC File Offset: 0x0001A5EC
			public Data(System.Drawing.Bitmap bitmap, bool canWrite)
			{
				this.FBitmap = bitmap;
                System.Drawing.Imaging.ImageLockMode flags = canWrite ? System.Drawing.Imaging.ImageLockMode.ReadWrite : System.Drawing.Imaging.ImageLockMode.ReadOnly;
				this.FBitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), flags, bitmap.PixelFormat);
			}

			// Token: 0x170000F5 RID: 245
			// (get) Token: 0x0600062D RID: 1581 RVA: 0x0001C434 File Offset: 0x0001A634
			public int ScanSize
			{
				get
				{
					return this.FBitmapData.Stride;
				}
			}

			// Token: 0x170000F6 RID: 246
			// (get) Token: 0x0600062E RID: 1582 RVA: 0x0001C441 File Offset: 0x0001A641
			public ReadOnlyMemory<byte> Bytes
			{
				get
				{
					return this.Memory;
				}
			}

			// Token: 0x0600062F RID: 1583 RVA: 0x0001C450 File Offset: 0x0001A650
			public override unsafe Span<byte> GetSpan()
			{
				return new Span<byte>(this.FBitmapData.Scan0.ToPointer(), this.FBitmapData.Height * this.FBitmapData.Stride);
			}

			// Token: 0x06000630 RID: 1584 RVA: 0x0001C48C File Offset: 0x0001A68C
			public override unsafe MemoryHandle Pin(int elementIndex = 0)
			{
				return new MemoryHandle(this.FBitmapData.Scan0.ToPointer(), default(GCHandle), this);
			}

			// Token: 0x06000631 RID: 1585 RVA: 0x0001C4BB File Offset: 0x0001A6BB
			public override void Unpin()
			{
			}

			// Token: 0x06000632 RID: 1586 RVA: 0x0001C4BD File Offset: 0x0001A6BD
			protected override void Dispose(bool disposing)
			{
				this.FBitmap.UnlockBits(this.FBitmapData);
			}

			// Token: 0x040002D5 RID: 725
			private readonly System.Drawing.Bitmap FBitmap;

			// Token: 0x040002D6 RID: 726
			private readonly System.Drawing.Imaging.BitmapData FBitmapData;
		}
	}
}
