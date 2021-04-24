using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace WinD.Models
{
    /*===================================================
    * 类名称: PlugResource
    * 类描述:插件资源模型
    * 创建人: musli
    * 创建时间: 2020/10/30 22:28:12
    * 修改人: 
    * 修改时间:
    * 版本： @version 1.0
    =====================================================*/
    public class PlugResource : ObservableObject
    {
        private string name;
        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private bool isEnable;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable
        {
            get => isEnable;
            set => SetProperty(ref isEnable, value);
        }
    }
}
