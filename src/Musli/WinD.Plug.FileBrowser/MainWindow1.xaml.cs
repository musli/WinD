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
using System.Windows.Interop;
using Win32;
using System.Data;
using System.Diagnostics;
using System.Threading;
using static Win32.User;
using WinD.Extended;
using System.ComponentModel;
using WF = System.Windows.Forms;
using WFI = System.Windows.Forms.Integration;
using System.Collections.ObjectModel;

namespace WinD.Plug.FileBrowser
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow1 : Page, INotifyPropertyChanged, IDisposable
    {
        public MainWindow1()
        {
            InitializeComponent();
            this.DataContext = this;
            HostPlugInterop.HostChanged += (command, para) =>
            {
                switch (command)
                {
                    case "OpacityChanged":
                        Panels.ToList().ForEach((u) =>
                        {
                            User.SetLayeredWindowAttributes((IntPtr)u.Panel.Tag, 0xffffff, (byte)Math.Ceiling(255 * (Convert.ToDouble(para))), 2);
                        });
                        break;
                        //case "WidthChanged":
                        //    Panels.ToList().ForEach((u) =>
                        //    {
                        //    User.SetWindowPos((IntPtr)u.Panel.Tag, (IntPtr)(0), -10, 0, (int)(Convert.ToDouble(para)), (int)(this.ActualHeight - 20), User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);
                        //    });
                        //    break;
                        //case "RowsChanged":
                        //case "ColumnsChanged":
                        //    Task.Run(() =>
                        //    {
                        //        Thread.Sleep(100);
                        //        User.SetWindowPos(browserHandle, (IntPtr)(0), 0, 0, (int)(this.ActualWidth + 20), (int)(this.ActualHeight - 20), User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);
                        //    });
                        //    break;
                }
            };
        }

        private IntPtr browserHandle = IntPtr.Zero;
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<ExplorerModel1> panels = new ObservableCollection<ExplorerModel1>();

        public ObservableCollection<ExplorerModel1> Panels
        {
            get { return panels; }
            set
            {
                panels = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Panels"));
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Panels.ToList().ForEach(u => u.IsSelected = false);
            var ex = new ExplorerModel1() { Panel = new WF.Panel(), IsSelected = true };
            Panels.Add(ex);

        }

        private void sliOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Panels.ToList().ForEach((u) =>
            {
                User.SetLayeredWindowAttributes((IntPtr)u.Panel.Tag, 0xffffff, (byte)sliOpacity.Value, 2);
            });
        }

        private void WindowsFormsHost_Loaded(object sender, RoutedEventArgs e)
        {
            var host = sender as WFI.WindowsFormsHost;
            var tempModel = host.DataContext as ExplorerModel1;
            var panel = tempModel.Panel;
            host.Child = panel;

            var curHandle = panel.Handle;
            Task.Run(() =>
            {
                Thread.Sleep(200);

                var temp = @"C:\Windows\explorer.exe";
                var tempProcess = Process.Start(temp, @"D:\.net5");
                for (int i = 0; i < 200; i++)
                {
                    Thread.Sleep(100);

                    browserHandle = (IntPtr)User.FindWindow("CabinetWClass", null);
                    panel.Tag = browserHandle;
                    if (browserHandle != IntPtr.Zero)
                        break;
                }
                ////消除边框
                //var style = User.GetWindowLong(browserHandle, User.GWL_STYLE);
                //style |= (int)User.WS_CHILD;
                //style |= (int)User.WS_CLIPCHILDREN;
                //User.SetWindowLong(browserHandle, User.GWL_STYLE, style);

                User.SetParent(browserHandle, curHandle);

                //消除边框
                var style = User.GetWindowLong(browserHandle, User.GWL_STYLE);
                style &= ~(int)(User.WS_EX_TOOLWINDOW | User.WS_CAPTION | User.WS_THICKFRAME |
                    User.WS_MINIMIZEBOX | User.WS_MAXIMIZEBOX | User.WS_MAXIMIZE | User.WS_SYSMENU);
                //style |= (int)User.WS_CHILD;
                //style |= (int)User.WS_CLIPCHILDREN;
                //style |= (int)User.WS_CHILD;
                //style |= (int)User.WS_CLIPCHILDREN;

                User.SetWindowLong(browserHandle, User.GWL_STYLE, style);
                //设置透明
                int exStyle = User.GetWindowLong(browserHandle, User.GWL_EXSTYLE);
                exStyle |= (int)User.WS_EX_LAYERED;

                User.SetWindowLong(browserHandle, User.GWL_EXSTYLE, exStyle);
                User.SetLayeredWindowAttributes(browserHandle, 0xffffff, 255, 2);

                //定位位置
                Dispatcher.Invoke(() =>
                {
                    User.SetWindowPos(browserHandle, (IntPtr)(0), 0, 0, (int)(host.ActualWidth), (int)(host.ActualHeight), User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);

                });
            });
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Panels.ToList().ForEach((u) =>
            {
                var handle = (IntPtr)u.Panel.Tag;
                User.SetWindowPos(handle, (IntPtr)(0), 0, 0, (int)(u.Panel.Width), (int)(u.Panel.Height), User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);
            });
        }

        private void btnJumpC_Click(object sender, RoutedEventArgs e)
        {
            var panel = Panels.FirstOrDefault(u => u.IsSelected)?.Panel;
            if (panel == null)
            {
                System.Windows.MessageBox.Show("当前未选中!");
                return;
            }

            int process = 0;
            User.GetWindowThreadProcessId((IntPtr)panel.Tag, ref process);
            var tempId = Process.GetProcessById(process);
            tempId.Kill();

            var curHandle = panel.Handle;
            var temp = @"C:\Windows\explorer.exe";
            var tempProcess = Process.Start(temp, @"C:\");
            for (int i = 0; i < 200; i++)
            {
                Thread.Sleep(100);

                browserHandle = (IntPtr)User.FindWindow("CabinetWClass", null);
                panel.Tag = browserHandle;
                if (browserHandle != IntPtr.Zero)
                    break;
            }

            User.SetParent(browserHandle, curHandle);
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
            User.SetWindowPos(browserHandle, (IntPtr)(0), 0, 0, (int)(panel.Width), (int)(panel.Height), User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);

        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var tempModel = Panels.Last();
            if (tempModel != null)
            {
                Panels.Remove(tempModel);

                int process = 0;
                User.GetWindowThreadProcessId((IntPtr)tempModel.Panel.Tag, ref process);
                var tempId = Process.GetProcessById(process);
                tempId.Kill();
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (Panels.Count > 0)
            {
                //释放占用的文件管理器进程
                Panels.ToList().ForEach((u) =>
                {
                    int process = 0;
                    User.GetWindowThreadProcessId((IntPtr)u.Panel.Tag, ref process);
                    var tempId = Process.GetProcessById(process);
                    tempId.Kill();

                    u.Panel.Dispose();
                });
                Panels.Clear();
            }
            Panels = null;
        }

    }

    public class ExplorerModel1 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        private WF.Panel panel;

        public WF.Panel Panel
        {
            get { return panel; }
            set
            {
                panel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Panel"));
            }
        }


    }
}
