using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace WinD.Extended
{
    /*===================================================
    * 类名称: IComponentService
    * 类描述: 组件服务
    * 创建人: musli
    * 创建时间: 2020/10/28 22:20:39
    * 修改人: 
    * 修改时间:
    * 版本： @version 1.0
    =====================================================*/
    public interface IComponentService : IDisposable
    {
        /// <summary>
        /// 插件名
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 主要页面
        /// </summary>
        public Page MainPage { get; }
        /// <summary>
        /// 关于页面
        /// </summary>
        public Page AboutPage { get; }

    }
    //[MetadataAttribute]
    //[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    //public class ExportMyInterfaceAttribute : Attribute, IComponentService
    //{
    //    public ExportMyInterfaceAttribute(string name, Page mainPage, Page aboutPage)

    //    {
    //        Name = name;
    //        MainPage = mainPage;
    //        AboutPage = aboutPage;
    //    }

    //    public string Name { get; set; }
    //    public Page MainPage { get; set; }
    //    public Page AboutPage { get; set; }
    //}
}
