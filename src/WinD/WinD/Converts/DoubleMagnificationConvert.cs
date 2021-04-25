/*===================================================
* 类名称: DoubleMagnificationConvert
* 类描述: 数字倍数转换器
* 创建人: musli
* 创建时间: 2020/10/30 11:37:12
* 修改人: 
* 修改时间:
* 版本： @version 1.0
=====================================================*/
using WinD.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Navigation;

namespace WinD.Converts
{
    /// <summary>
    /// 数字倍数转换器
    /// </summary>
    public class DoubleMagnificationConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var para = System.Convert.ToInt32(parameter);
            var tempValue = System.Convert.ToDouble( value);
            if (para < 0)
                return tempValue / Math.Abs(para);
            else
                return tempValue * para;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var para = System.Convert.ToInt32(parameter);
            var tempValue = System.Convert.ToDouble(value);
            if (para < 0)
                return tempValue * Math.Abs(para);
            else
                return tempValue / para;
        }
    }
}
