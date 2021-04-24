
/*===================================================
* 类名称: EdgeComponent
* 类描述: MSEdge插件
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

namespace WinD.Plug.Edge
{
    /// <summary>
    /// 百度组件对接类
    /// </summary>
    [Export("Html", typeof(IComponentService))]
    public class EdgeComponent : IComponentService
    {
        public Page MainPage { get => new MainPage(); }

        public Page AboutPage { get => new AboutPage(); }

        public string Name { get => "桌面Edge"; }

        public void Dispose()
        {
        }
    }
}
