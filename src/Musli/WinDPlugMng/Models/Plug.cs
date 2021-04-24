/*===================================================
* 类名称: Menu
* 类描述: 
* 创建人: musli
* 创建时间: 2020/10/19 17:27:27
* 修改人: 
* 修改时间:
* 版本： @version 1.0
=====================================================*/
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace WinDPlugMng.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Plug:ObservableObject
    {
        private string name;
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private Uri url;
        /// <summary>
        /// 页面地址
        /// </summary>
        public Uri Url
        {
            get { return url; }
            set { SetProperty(ref url, value); }
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
