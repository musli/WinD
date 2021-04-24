using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Musli.Plug_ins.Browser
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        ChromiumWebBrowser chrome;
        public MainPage()
        {
            InitializeComponent();
            chrome = new ChromiumWebBrowser("http://lab.mkblog.cn/FCGames/#/");
            ContentGrid.Children.Add(chrome);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            chrome.Address = txtAdderss.Text;
        }
    }
}
