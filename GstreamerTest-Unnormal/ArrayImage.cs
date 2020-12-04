using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GstreamerTest_Unnormal
{
	// Token: 0x0200002B RID: 43
	public class ArrayImage<T> : IImage where T : struct
	{
		// Token: 0x06000198 RID: 408 RVA: 0x00011C58 File Offset: 0x0000FE58
		public ArrayImage(T[] data, ImageInfo info, bool isVolatile) : this(data, info, isVolatile, info.ScanSize)
		{
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00011C6A File Offset: 0x0000FE6A
		public ArrayImage(T[] data, ImageInfo info, bool isVolatile, int scanSize)
		{
			this.FData = data;
			this.Info = info;
			this.IsVolatile = isVolatile;
			this.FScanSize = scanSize;
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600019A RID: 410 RVA: 0x00011C8F File Offset: 0x0000FE8F
		public ImageInfo Info { get; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600019B RID: 411 RVA: 0x00011C97 File Offset: 0x0000FE97
		public bool IsVolatile { get; }

		// Token: 0x0600019C RID: 412 RVA: 0x00011C9F File Offset: 0x0000FE9F
		public IImageData GetData()
		{
			return new ArrayImage<T>.Data(this.FData, this.FScanSize);
		}

		// Token: 0x04000080 RID: 128
		public static readonly ArrayImage<T> Default = new ArrayImage<T>(Array.Empty<T>(), default(ImageInfo), false);

		// Token: 0x04000081 RID: 129
		private readonly T[] FData;

		// Token: 0x04000082 RID: 130
		private readonly int FScanSize;

		// Token: 0x02000113 RID: 275
		private class Data : MemoryManager<byte>, IImageData, IDisposable
		{
			// Token: 0x06000625 RID: 1573 RVA: 0x0001C398 File Offset: 0x0001A598
			public Data(T[] array, int scanSize)
			{
				this.FMemory = array;
				this.ScanSize = scanSize;
			}

			// Token: 0x170000F3 RID: 243
			// (get) Token: 0x06000626 RID: 1574 RVA: 0x0001C3B3 File Offset: 0x0001A5B3
			public int ScanSize { get; }

			// Token: 0x170000F4 RID: 244
			// (get) Token: 0x06000627 RID: 1575 RVA: 0x0001C3BB File Offset: 0x0001A5BB
			public ReadOnlyMemory<byte> Bytes
			{
				get
				{
					return this.Memory;
				}
			}

			// Token: 0x06000628 RID: 1576 RVA: 0x0001C3C8 File Offset: 0x0001A5C8
			public override Span<byte> GetSpan()
			{
				return MemoryMarshal.AsBytes<T>(this.FMemory.Span);
			}

			// Token: 0x06000629 RID: 1577 RVA: 0x0001C3DA File Offset: 0x0001A5DA
			public override MemoryHandle Pin(int elementIndex = 0)
			{
				return this.FMemory.Pin();
			}

			// Token: 0x0600062A RID: 1578 RVA: 0x0001C3E7 File Offset: 0x0001A5E7
			public override void Unpin()
			{
			}

			// Token: 0x0600062B RID: 1579 RVA: 0x0001C3E9 File Offset: 0x0001A5E9
			protected override void Dispose(bool disposing)
			{
			}

			// Token: 0x040002D3 RID: 723
			private readonly Memory<T> FMemory;
		}
	}
}
