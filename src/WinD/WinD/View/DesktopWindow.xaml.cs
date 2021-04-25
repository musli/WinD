using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using Win32;
using WinD.Common.Extensions;
using WinD.Extended;
using WinD.Models;
using WinD.ViewModel;
using static Win32.User;

namespace WinD.View
{
    /// <summary>
    /// DesktopWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DesktopWindow : Window
    {
        #region Fields
        /// <summary>
        /// VM
        /// </summary>
        private DesktopWindowViewModel viewModel;
        /// <summary>
        /// 暂时没用的句柄
        /// </summary>
        private static IntPtr iconPtr = IntPtr.Zero;
        /// <summary>
        /// 暂时没用的句柄
        /// </summary>
        private static IntPtr iconParentPtr = IntPtr.Zero;
        /// <summary>
        /// 工作区宽度
        /// </summary>
        private static double workAreWidth = SystemParameters.WorkArea.Width;
        /// <summary>
        /// 工作区高度
        /// </summary>
        private static double workAreHeight = SystemParameters.WorkArea.Height;
        /// <summary>
        /// 窗体拖动计数器
        /// </summary>
        private int Count = 0;
        /// <summary>
        /// 窗体拖动后台任务
        /// </summary>
        private Task changeWidthTask;
        #endregion

        #region Propertys

        #endregion

        #region Methods
        public DesktopWindow()
        {
            InitializeComponent();
            //初始化窗体滑块的取值
            sliSize.Maximum = workAreWidth;
            sliSize.Value = workAreWidth / 2;
            //初始化VM
            viewModel = App.Current.MSService.GetService<DesktopWindowViewModel>();
            this.DataContext = viewModel;
            borNotifly.DataContext = viewModel;
            //老板键 后期加钩子做成全局的，目前只有在本窗体有焦点的时候有效
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, (_1, _2) =>
            {
                sliOpacity.Value = Opacity == 0 ? 1 : 0;
            }));
        }
        public static void SetProgramToWorkerW(IntPtr curPtr)
        {
            IntPtr progman = (IntPtr)User.FindWindow("Progman", null);
            int unusefulResult = 0;
            //发送0x052C 创建一个WorkerW窗体
            User.SendMessageTimeout(progman, (int)0x052C, (int)new IntPtr(0), (int)IntPtr.Zero, (int)0x0000, (int)1000, ref unusefulResult);
            IntPtr workerw = IntPtr.Zero;
            var enumWindowResult = (IntPtr)User.EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) =>
            {
                //如果其子窗体包含SHELLDLL_DefView，则记录为桌面WorkerW，使用0x052C创建的WorkerW必然在桌面WorkerW的后一个窗体
                iconParentPtr = (IntPtr)User.FindWindowEx(tophandle, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (iconParentPtr != IntPtr.Zero)
                {
                    //根据找到的Worker去查找下一个名叫WorkerW兄弟窗口
                    var sis = (IntPtr)User.FindWindowEx(IntPtr.Zero, tophandle, "WorkerW", null);
                    workerw = iconParentPtr;

                    //处理alt+tab可以看见本程序
                    //int exStyle = User.GetWindowLong(curPtr, User.GWL_EXSTYLE);
                    //exStyle |= (int)WS_EX_TOOLWINDOW;
                    //var style = User.GetWindowLong(curPtr,User.GWL_STYLE);

                    //必须在设置父容器后设置WS_Clhild,
                    //1.是可以防止System.InvalidOperationException:“Hosted HWND must be a child window of the specified parent.”
                    //2.重置窗体使其可以拖动
                    //var style = User.WS_VISIBLE | User.WS_CHILD;
                    //User.SetWindowLong(curPtr, User.GWL_STYLE, style);

                    //改变样式
                    User.ShowWindow(curPtr, User.SW_SHOW);
                    User.EnableWindow(curPtr, 1);
                    int style = User.GetWindowLong(curPtr, User.GWL_STYLE);
                    style |= ((int)User.WS_CHILD);
                    style |= ((int)User.WS_CLIPCHILDREN);
                    User.SetWindowLong(curPtr, User.GWL_STYLE, style);

                    //使程序和桌面图标拥有共同的父容器
                    User.SetParent(curPtr, workerw);
                    User.SetWindowPos(curPtr, (IntPtr)(0), 0, 0, (int)workAreWidth / 2, (int)workAreHeight, 0x0040);



                    //User.SetWindowLong(curPtr, User.GWL_EXSTYLE, exStyle);
                    //User.SetWindowLong(curPtr, User.GWL_STYLE, style);

                    //移动图标窗体到程序的右边
                    iconPtr = (IntPtr)User.FindWindowEx(iconParentPtr, IntPtr.Zero, "SysListView32", null);
                    User.SetWindowPos(iconPtr, (IntPtr)(0), (int)workAreWidth / 2, 0, (int)workAreWidth / 2, (int)workAreHeight, User.SWP_SHOWWINDOW | User.SWP_NOACTIVATE);

                    return 0;
                }
                return 1;
            }), (int)IntPtr.Zero);
        }

        /// <summary>
        /// 重新加载插件到内存
        /// </summary>
        private void LoadPlug()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\Plug\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var dis = Directory.EnumerateDirectories(path);
            var assemblies = new List<Assembly>();
            foreach (var item in dis)
            {
                var dir = new DirectoryInfo(item + "\\net5.0-windows");
                var files = dir.GetFiles();
                foreach (var fileItem in files)
                {
                    if (fileItem.Name.StartsWith("WinD.Plug.") && ".dll".Equals(fileItem.Extension))
                    {
                        assemblies.Add(Assembly.LoadFrom(fileItem.FullName));
                    }
                }
            }

            var conventions = new ConventionBuilder();
            conventions.ForTypesDerivedFrom<IComponentService>().Export<IComponentService>().Shared();

            var configuration = new ContainerConfiguration().WithAssemblies(assemblies, conventions);
            var container = configuration.CreateContainer();
            App.Current.Services = container.GetExports<IComponentService>("Html");
            var temp = App.Current.Services.Count();

        }
        #endregion

        #region Events
        /// <summary>
        /// 窗体加载事件，将窗体嵌入桌面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void win_Loaded(object sender, RoutedEventArgs e)
        {
            //加载配置文件
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "data.json"))
            {
                viewModel.Config = JsonConvert.DeserializeObject<UserConfig>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "data.json"));
                if (viewModel.Config.WindowWidth > 0)
                    workAreWidth = viewModel.Config.WindowWidth * 2;
            }

            if (viewModel.Config.MyPlugResources == null)
                viewModel.Config.MyPlugResources = new ObservableCollection<PlugResource>();

            //如果存在未经过插件管理器导入的插件则也将其合并
            var plugPath = AppDomain.CurrentDomain.BaseDirectory + @"Plug\";
            if (!Directory.Exists(plugPath))
                Directory.CreateDirectory(plugPath);
            var tempPlugs = Directory.GetDirectories(plugPath).Select(u => new PlugResource() { Name = u.Substring(u.LastIndexOf(@"\") + 1) });
            var tempResource = viewModel.Config.MyPlugResources;
            viewModel.Config.MyPlugResources = tempPlugs.ToObservableCollection();
            tempResource.ForEach((u) =>
            {
                var model = viewModel.Config.MyPlugResources.Where(k => u.Name.Equals(k.Name)).FirstOrDefault();
                if (model != null)
                    model.IsEnable = u.IsEnable;
            });

            //读取上一次保存的配置区域的高度
            grid.RowDefinitions[2].Height = new GridLength(viewModel.Config.ConfigContentHeight);

            //加载插件到VM
            LoadPlug();
            foreach (var item in viewModel.Config.MyPlugResources)
            {
                var tempPlug = App.Current.Services;
                if (item.IsEnable && App.Current.Services.Any(u => u.Name.Equals(item.Name)))
                    viewModel.Plugs.Add(App.Current.Services.First(u => u.Name.Equals(item.Name)));
            }
            //此处看情况需要延迟一点时间，等待插件在界面上初始化完毕.否则使<Page/>里面的<WindowsFormsHost/>不能成功创建
            //获取程序窗体的句柄
            var help = new WindowInteropHelper(this);
            var curHandle = help.Handle;
            ThisHwnd = curHandle;
            Task.Run(() =>
            {
                //处理alt+tab可以看见本程序
                int exStyle = User.GetWindowLong(curHandle, User.GWL_EXSTYLE);
                exStyle |= (int)WS_EX_TOOLWINDOW;
                User.SetWindowLong(curHandle, User.GWL_EXSTYLE, exStyle);


                //嵌入程序窗体到桌面
                SetProgramToWorkerW(curHandle);
            });

        }
        IntPtr ThisHwnd = IntPtr.Zero;
        /// <summary>
        /// 屏蔽Alt+F4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// 窗体宽度滑块拖动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Width = sliSize.Value;
            if (changeWidthTask == null)
            {
                User.SetWindowPos(iconPtr, (IntPtr)(0), 0, 0, 0, (int)workAreHeight, 0x0040);
                Count = 3;
                changeWidthTask = new Task(() =>
                {
                    do
                    {
                        Thread.Sleep(100);
                        Count--;
                    } while (Count > 0);
                });
                changeWidthTask.Start();
                changeWidthTask.ContinueWith(new Action<Task>((task) =>
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        var width = (int)(workAreWidth - this.Width);
                        var left = (int)this.Width;
                        User.SetWindowPos(iconPtr, (IntPtr)(0), left, 0, width, (int)workAreHeight, 0x0040);
                        viewModel.Config.WindowWidth = this.Width;
                    }));
                    changeWidthTask = null;
                }));
            }
            else
            {
                Count = 3;
            }

            HostPlugInterop.OnHostChanged("WidthChanged", this.Width);
        }

        /// <summary>
        /// 强制重新布局Windows桌面 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReLoad_Click(object sender, RoutedEventArgs e)
        {
            ////128
            //int exStyle = User.GetWindowLong(ThisHwnd, User.GWL_EXSTYLE);
            ////1342177280
            //int style = User.GetWindowLong(ThisHwnd, User.GWL_STYLE);

            var host = new System.Windows.Forms.Integration.WindowsFormsHost();
            grid.Children.Add(host);

            return;
            var process = Process.GetProcessesByName("explorer");
            foreach (Process item in process)
                item.Kill();
        }

        /// <summary>
        /// 启用插件时，自动导航到其内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            var frame = sender.To<Frame>();
            if (frame.Tag == null)
            {
                frame.Style = null;
                //var model = frame.DataContext.To<ContentPresenter>().Content.To<IComponentService>();
                var model = frame.DataContext.To<IComponentService>();
                if (model != null)
                {
                    frame.Navigate(model.MainPage);
                    frame.Tag = "True";
                }
            }

        }

        /// <summary>
        /// 启用插件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var toggle = sender.To<ToggleButton>();
            var text = toggle.DataContext.To<PlugResource>().Name;
            viewModel.Plugs.Add(App.Current.Services.First(u => u.Name.Equals(text)));
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "data.json", JsonConvert.SerializeObject(viewModel.Config));
        }

        /// <summary>
        /// 停用插件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var toggle = sender.To<ToggleButton>();
            var text = toggle.DataContext.To<PlugResource>().Name;
            var tempPlug = App.Current.Services.First(u => u.Name.Equals(text));
            viewModel.Plugs.Remove(tempPlug);
            tempPlug.Dispose();
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "data.json", JsonConvert.SerializeObject(viewModel.Config));
        }
        /// <summary>
        /// 配置去分割条拖动完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridSplitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            viewModel.Config.ConfigContentHeight = grid.RowDefinitions[2].ActualHeight;
        }
        /// <summary>
        /// 修改透明度的时候发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliOpacity_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            HostPlugInterop.OnHostChanged("OpacityChanged", this.Opacity);
        }
        /// <summary>
        /// 行改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliRows_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HostPlugInterop.OnHostChanged("RowsChanged", sliRows.Value);
        }
        /// <summary>
        /// 列改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sliColumns_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HostPlugInterop.OnHostChanged("ColumnsChanged", sliColumns.Value);
        }
        /// <summary>
        /// 插件管理按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMng_Click(object sender, RoutedEventArgs e)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "WinDPlugMng.exe";
            if (File.Exists(path))
                Process.Start(path);

            this.btnExit_Click(btnExit, null);
        }
        /// <summary>
        /// 退出按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var temp = sender.To<Button>().DataContext;
            //保存配置
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "data.json", JsonConvert.SerializeObject(viewModel.Config));
            //重置桌面
            User.SetWindowPos(iconPtr, (IntPtr)(0), 0, 0, (int)workAreWidth, (int)workAreHeight, 0x0040);
            //释放托盘
            NotifyIconContextContent.Dispose();
            //退出进程
            Process.GetCurrentProcess().Kill();
        }
        #endregion

    }
}
