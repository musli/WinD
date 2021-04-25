/*===================================================
* 类名称: DesktopWindowViewModel
* 类描述: 桌面VM
* 创建人: musli
* 创建时间: 2020/10/30 10:57:25
* 修改人: 
* 修改时间:
* 版本： @version 1.0
=====================================================*/
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using WinD.Models;
using WinD.Extended;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Controls;

namespace WinD.ViewModel
{
    /// <summary>
    /// 桌面VM
    /// </summary>
    public class DesktopWindowViewModel : ObservableObject
    {
        public DesktopWindowViewModel()
        {
            //ApplicationCommands.SaveCommand = new RelayCommand(() => { }, () =>
            //{
            //    return false;
            //});
        }

        private ObservableCollection<IComponentService> plugs = new ObservableCollection<IComponentService>();
        /// <summary>
        /// 我的插件集合
        /// </summary>
        public ObservableCollection<IComponentService> Plugs
        {
            get => plugs;
            set => SetProperty(ref plugs, value);
        }
        private UserConfig config = new UserConfig();
        /// <summary>
        /// 我的插件资源集合
        /// </summary>
        public UserConfig Config
        {
            get => config;
            set => SetProperty(ref config, value);
        }

    }
    /// <summary>
    /// 用户配置类
    /// </summary>
    public class UserConfig : ObservableObject
    {
        private double windowOpacity = 10;
        /// <summary>
        /// 窗体透明度
        /// </summary>
        public double WindowOpacity
        {
            get { return windowOpacity; }
            set { SetProperty(ref windowOpacity, value); }
        }
        private double windowWidth=960;
        /// <summary>
        /// 窗体宽度
        /// </summary>
        public double WindowWidth
        {
            get { return windowWidth; }
            set { SetProperty(ref windowWidth, value); }
        }
        private double configContentHeight=200;
        /// <summary>
        /// 配置区域内容高度
        /// </summary>
        public double ConfigContentHeight
        {
            get { return configContentHeight; }
            set { SetProperty(ref configContentHeight, value); }
        }
        private int contentRows=2;
        /// <summary>
        /// 内容行数
        /// </summary>
        public int ContentRows
        {
            get { return contentRows; }
            set { SetProperty(ref contentRows, value); }
        }
        private int contentColumns=1;
        /// <summary>
        /// 内容列数
        /// </summary>
        public int ContentColumns
        {
            get { return contentColumns; }
            set { SetProperty(ref contentColumns, value); }
        }

        private ObservableCollection<PlugResource> myPlugResources = new ObservableCollection<PlugResource>();
        /// <summary>
        /// 我的插件资源集合
        /// </summary>
        public ObservableCollection<PlugResource> MyPlugResources
        {
            get => myPlugResources;
            set => SetProperty(ref myPlugResources, value);
        }
    }
}
