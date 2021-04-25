using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Interop;
using Win32;
using System.Diagnostics;
using System.Threading;
using static Win32.User;
using WinD.Extended;
using WinD.Common;

namespace WinD.Plug.Edge
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            HostPlugInterop.HostChanged += (command, para) =>
            {
                switch (command)
                {
                    case "OpacityChanged":
                        User.SetLayeredWindowAttributes(browserHandle, 0xffffff, (byte)Math.Ceiling(255 * (Convert.ToDouble(para))), 2);
                        break;
                    case "WidthChanged":
                        User.SetWindowPos(browserHandle, (IntPtr)(0), -10, 0, (int)(Convert.ToDouble(para)), (int)(this.ActualHeight - 20), User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);
                        break;
                    case "RowsChanged":
                    case "ColumnsChanged":
                        Task.Run(() =>
                        {
                            Thread.Sleep(100);
                            User.SetWindowPos(browserHandle, (IntPtr)(0), -10, 0, (int)(this.ActualWidth + 20), (int)(this.ActualHeight - 20), User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);
                        });
                        break;
                }
            };
        }

        private IntPtr browserHandle = IntPtr.Zero;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var temp = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
            var tempProcess = Process.Start(temp);
            for (int i = 0; i < 200; i++)
            {
                Thread.Sleep(100);
                var enumWindowResult = (IntPtr)User.EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) =>
                {
                    var strBuilder = new StringBuilder();
                    User.GetWindowText(tophandle, strBuilder, 255);
                    Debug.WriteLine(strBuilder.ToString());
                    if (strBuilder.ToString().EndsWith("Microsoft? Edge"))
                    {
                        browserHandle = tophandle;
                        return 0;
                    }
                    return 1;
                }), (int)IntPtr.Zero);
                if (browserHandle != IntPtr.Zero)
                    break;
            }

            //嵌入wpf程序
            EmbeddedApp tempApp = new EmbeddedApp(browserHandle);
            borContent.Child = tempApp;


            //消除边框
            var style = User.GetWindowLong(browserHandle, User.GWL_STYLE);
            style &= ~(int)(User.WS_EX_TOOLWINDOW | User.WS_CAPTION | User.WS_THICKFRAME |
                User.WS_MINIMIZEBOX | User.WS_MAXIMIZEBOX | User.WS_MAXIMIZE | User.WS_SYSMENU);
            User.SetWindowLong(browserHandle, User.GWL_STYLE, style);

            //设置透明
            int exStyle = User.GetWindowLong(browserHandle, User.GWL_EXSTYLE);
            exStyle |= (int)User.WS_EX_LAYERED;
            User.SetWindowLong(browserHandle, User.GWL_EXSTYLE, exStyle);
            User.SetLayeredWindowAttributes(browserHandle, 0xffffff, 128, 2);

            //定位位置
            User.SetWindowPos(browserHandle, (IntPtr)(0), -10, 0, (int)(this.ActualWidth + 20), (int)(this.ActualHeight - 20), 0x0040);



        }

        private void sliOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            User.SetLayeredWindowAttributes(browserHandle, 0xffffff, (byte)sliOpacity.Value, 2);
        }
    }
}
