using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;
using Win32;

namespace WinD.Common
{
    public class EmbeddedApp : HwndHost, IKeyboardInputSink
    {
        private IntPtr TargetHWND;

        public EmbeddedApp(IntPtr intPtr)
        {
            TargetHWND = intPtr;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {

            //改变样式
            //User.ShowWindow(TargetHWND, User.SW_SHOW);
            //User.EnableWindow(TargetHWND, 1);
            int style = User.GetWindowLong(TargetHWND, User.GWL_STYLE);
            //style = style & ~((int)User.WS_CAPTION) & ~((int)User.WS_THICKFRAME);
            style |= ((int)User.WS_CHILD);
            style |= ((int)User.WS_CLIPCHILDREN);
            User.SetWindowLong(TargetHWND, User.GWL_STYLE, User.WS_CHILD);

            //嵌入进去
            User.SetParent(TargetHWND, hwndParent.Handle);
            return new HandleRef(this, TargetHWND);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            Console.WriteLine("释放了");
        }
    }

}
