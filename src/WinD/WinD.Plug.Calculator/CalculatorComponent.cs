/*===================================================
* 类名称: CalculatorComponent
* 类描述: 计算器插件
* 创建人: musli
* 创建时间: 2020/10/30 16:48:37
* 修改人: 
* 修改时间:
* 版本： @version 1.0
=====================================================*/
using WinD.Extended;
using Musli.Plug_ins.Calculator;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Musli.Plug_ins.CalculatorComponent
{

    [Export("Html", typeof(IComponentService))]
    public class CalculatorComponent : IComponentService
    {
        public Page MainPage { get => new MainPage(); }

        public Page AboutPage { get => new AboutPage(); }

        public string Name { get => "简单计算器"; }

        public void Dispose()
        {
        }
    }
}
