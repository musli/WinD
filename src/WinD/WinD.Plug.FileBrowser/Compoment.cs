/*===================================================
* 类名称: Compoment
* 类描述: 文件浏览器插件
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

namespace WinD.Plug.FileBrowser
{
    /// <summary>
    /// 文件浏览器组件对接类
    /// </summary>
    [Export("Html", typeof(IComponentService))]
    public class Compoment : IComponentService
    {
        private Page mainPage;

        public Page MainPage
        {
            get
            {
                if (mainPage == null)
                    mainPage = new MainPage();
                return mainPage;
            }
        }
        private Page aboutPage;

        public Page AboutPage
        {
            get
            {
                if (aboutPage == null)
                    aboutPage = new AboutPage();
                return aboutPage;
            }
        }

        public string Name { get => "桌面文件浏览器"; }

        public void Dispose()
        {
            if (mainPage != null)
            {
                ((MainPage)mainPage).Dispose();
                mainPage = null;
            }
            if(aboutPage!=null)
                mainPage = null;
        }
    }
}
