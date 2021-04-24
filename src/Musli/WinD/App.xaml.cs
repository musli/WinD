using Microsoft.Extensions.DependencyInjection;
using WinD.ViewModel;
using WinD.Common.Extensions;
using WinD.Extended;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Composition;

namespace WinD
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 获取当前正在使用中的应用程序实例
        /// </summary>
        public new static App Current => (App)Application.Current;
        /// <summary>
        /// 实例来解析应用程序服务
        /// </summary>
        public IServiceProvider MSService { get; }


        public App()
        {
            var services = new ServiceCollection();
            services.AddTransient<DesktopWindowViewModel>();
            MSService = services.BuildServiceProvider();

            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //UI线程未捕获异常处理事件(UI主线程)
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            //非UI线程未捕获异常处理事件(例如自己创建的一个子线程)
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject.To<Exception>();
            MessageBox.Show(ex.Message + "\n\r" + ex.StackTrace);
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var ex = e.Exception;
            MessageBox.Show(ex.Message + "\n\r" + ex.StackTrace);
            e.Handled = true;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var ex = e.Exception;
            MessageBox.Show(ex.Message + "\n\r" + ex.StackTrace);
            
        }


        [ImportMany("Html")]
        public IEnumerable<IComponentService> Services { get; set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.Compose();
        }
        private void Compose()
        {
            //var path = AppDomain.CurrentDomain.BaseDirectory + "\\Plug\\";
            //var dis =  Directory.EnumerateDirectories(path);
            //var dllList = new List<ComposablePartCatalog>();
            //foreach (var item in dis)
            //{
            //    var dir = new DirectoryInfo(item+ "\\netcoreapp3.1");
            //    var files = dir.GetFiles();
            //    foreach (var fileItem in files)
            //    {
            //        if (fileItem.Name.StartsWith("Musli.Plug-ins.") && ".dll".Equals(fileItem.Extension))
            //        {
            //            dllList.Add(new AssemblyCatalog(Assembly.LoadFrom(fileItem.FullName)));
            //        }
            //    }
            //}
            //var agglog = new AggregateCatalog(dllList);
            //CompositionContainer container = new CompositionContainer(agglog);
            //container.ComposeParts(this);
        }




    }
}
