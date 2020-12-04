using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GstreamerTest_Unnormal
{
    public interface IImage
    {
        // Token: 0x1700002A RID: 42
        // (get) Token: 0x060001C2 RID: 450
        ImageInfo Info { get; }

        // Token: 0x060001C3 RID: 451
        IImageData GetData();

        // Token: 0x1700002B RID: 43
        // (get) Token: 0x060001C4 RID: 452
        bool IsVolatile { get; }
    }
}
