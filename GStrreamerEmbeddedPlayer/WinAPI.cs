using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GStrreamerEmbeddedPlayer
{
   internal class WinAPI
    {
        /* 
    * 声明 Win32 API 
    */
        [DllImport("user32.dll")]
       public static extern IntPtr SetParent(IntPtr hWndChild,
          IntPtr hWndNewParent
        );
        [DllImport("user32.dll")]
       public static extern Int32 GetWindowLong(IntPtr hWnd,
          Int32 nIndex
        );
        [DllImport("user32.dll")]
       public static extern Int32 SetWindowLong(IntPtr hWnd,
          Int32 nIndex,
          Int32 dwNewLong
        );
        [DllImport("user32.dll")]
       public static extern Int32 SetWindowPos(IntPtr hWnd,
          IntPtr hWndInsertAfter,
          Int32 X,
          Int32 Y,
          Int32 cx,
          Int32 cy,
          UInt32 uFlags
        );
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        /* 
            * 定义 Win32 常数 
            */
        public const Int32 GWL_STYLE = -16;
        public const Int32 WS_BORDER = (Int32)0x00800000L;
        public const Int32 WS_THICKFRAME = (Int32)0x00040000L;
        public const Int32 SWP_NOMOVE = 0x0002;
        public const Int32 SWP_NOSIZE = 0x0001;
        public const Int32 SWP_NOZORDER = 0x0004;
        public const Int32 SWP_FRAMECHANGED = 0x0020;
        public const Int32 SW_MAXIMIZE = 3;
        public IntPtr HWND_NOTOPMOST = new IntPtr(-2);




    }
}
