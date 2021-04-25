/*===================================================
* 类名称: BaiduComponent
* 类描述: 浏览器插件
* 创建人: musli
* 创建时间: 2020/10/29 8:48:37
* 修改人: 
* 修改时间:
* 版本： @version 1.0
=====================================================*/
using WinD.Extended;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using System.Windows.Controls;

namespace Musli.Plug_ins.Browser
{
    /// <summary>
    /// 百度组件对接类
    /// </summary>
    [Export("Html", typeof(IComponentService))]
    public class BaiduComponent : IComponentService
    {
        public Page MainPage { get => new MainPage(); }

        public Page AboutPage { get => new AboutPage(); }

        public string Name { get => "桌面百度"; }

        public void Dispose()
        {
        }
    }
}
