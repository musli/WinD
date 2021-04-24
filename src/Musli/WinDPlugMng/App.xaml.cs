using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WinDPlugMng
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
            services.AddTransient<MainWindowViewModel>();
            MSService = services.BuildServiceProvider();

        }
    }
}
