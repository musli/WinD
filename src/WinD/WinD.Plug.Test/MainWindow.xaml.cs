using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Win32;
//using WinD.Plug.Edge;
using WinD.Plug.FileBrowser;
//using WinD.Plug.SystemMonitoring;

namespace WinD.Plug.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainPage mainPage = new MainPage();
        //MainWindow1 mainPage1 = new MainWindow1();
        public MainWindow()
        {
            InitializeComponent();
            frame.Navigate(mainPage);
            //frame.Navigate(mainPage1);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //mainPage.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var temp = Process.GetProcessesByName("explorer");
            foreach (var item in temp)
            {
                item.Kill();
            }
        }
    }
}
