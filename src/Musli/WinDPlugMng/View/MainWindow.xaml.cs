using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WinD.Common.Extensions;
using WinDPlugMng.Models;

namespace WinDPlugMng
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                var vm = App.Current.MSService.GetService<MainWindowViewModel>();
                DataContext = vm;
                this.Reload();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Reload()
        {
            var temp = AppDomain.CurrentDomain.BaseDirectory + @"Plug\";
            var tempPlugs = Directory.GetDirectories(temp).Select(u => new Plug() { Name = u.Substring(u.LastIndexOf(@"\") + 1) });
            this.DataContext.To<MainWindowViewModel>().Plugs = tempPlugs.ToObservableCollection();
        }
        /// <summary>
        /// 删除插件点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var plugName = sender.To<Button>().DataContext.To<Plug>().Name;
            try
            {
                Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + @"Plug\" + plugName, true);
                MessageBox.Show("删除成功!");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("删除插件时出现错误:" + ex.Message);
            }
            finally
            {
                Reload();
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool isExists = false;
                var dir = new DirectoryInfo(dialog.SelectedPath + "\\net5.0-windows");
                var files = dir.GetFiles();
                foreach (var fileItem in files)
                    if (fileItem.Name.StartsWith("WinD.Plug.") && ".dll".Equals(fileItem.Extension))
                        isExists = true;
                if (isExists)
                {
                    CopyDirectory(dialog.SelectedPath, AppDomain.CurrentDomain.BaseDirectory + "Plug\\");
                    //File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "data.json", JsonConvert.SerializeObject(VM.Config));
                    Reload();
                    MessageBox.Show("导入成功!");
                }
                else
                {
                    MessageBox.Show("不存在插件文件！");
                }
            }
        }
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="srcdir"></param>
        /// <param name="desdir"></param>
        private void CopyDirectory(string srcdir, string desdir)
        {
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1);
            string desfolderdir = desdir + "\\" + folderName;

            if (desdir.LastIndexOf("\\") == (desdir.Length - 1))
                desfolderdir = desdir + folderName;

            string[] filenames = Directory.GetFileSystemEntries(srcdir);
            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }
                    CopyDirectory(file, desfolderdir);
                }
                else // 否则直接copy文件
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);
                    srcfileName = desfolderdir + "\\" + srcfileName;
                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }
                    File.Copy(file, srcfileName, true);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var process = Process.GetProcessesByName("WinD");
            if (process.Length == 0)
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + "WinD.exe";
                if (File.Exists(path))
                    Process.Start(path);
            }
        }
    }
}
