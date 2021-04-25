using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WinD.Plug.SystemMonitoring
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int x = 0;
            int y = 0;
            List<Point> list = new List<Point>();
            Task.Run(() =>
            {
                while (x < 200)
                {
                    Thread.Sleep(100);
                    y = DeskHelper.GetDeskUseRate();
                    if (x == 10)
                        y = 11;
                    if (x == 20)
                        y = 100;
                    if (x == 30)
                        y = 0;
                    if (x == 40)
                        y = 99;
                    list.Add(new Point(x, y));
                    x += 10;
                    Dispatcher.Invoke(() =>
                    {
                        DeskDc.Refresh(list);
                        MemonryDc.Refresh(list);
                        CPUDc.Refresh(list);
                        NetWorkDc.Refresh(list);
                        Debug.WriteLine(string.Join(" ",list.Select(u=>u.Y)));
                    });
                }
            });
        }
    }
}
