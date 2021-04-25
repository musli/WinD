using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WinDPlugMng.Models;

namespace WinDPlugMng
{
    public class MainWindowViewModel : ObservableObject
    {
        private ObservableCollection<Plug> plugs = new ObservableCollection<Plug>();
        /// <summary>
        /// 我的插件集合
        /// </summary>
        public ObservableCollection<Plug> Plugs
        {
            get => plugs;
            set => SetProperty(ref plugs, value);
        }
    }
}
