using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GstreamerTest_Unnormal
{
	// Token: 0x02000033 RID: 51
	public interface IImageData : IDisposable
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060001C5 RID: 453
		ReadOnlyMemory<byte> Bytes { get; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060001C6 RID: 454
		int ScanSize { get; }
	}
}
