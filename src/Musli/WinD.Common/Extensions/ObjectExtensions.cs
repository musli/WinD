/*===================================================
* 类名称: ObjectExtensions
* 类描述: Object 集合的扩展方法类
* 创建人: musli
* 创建时间: 2020/10/30 11:41:05
* 修改人: 
* 修改时间:
* 版本： @version 1.0
=====================================================*/
using System;
using System.Collections.Generic;
using System.Text;

namespace WinD.Common.Extensions
{
    /// <summary>
    /// Object 集合的扩展方法类
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 把对象类型转化为指定类型
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败引发异常。 </returns>
        public static T To<T>(this object value)
        {
            return (T)value;
        }

        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回指定的默认值
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <param name="defaultValue"> 转化失败返回的指定默认值 </param>
        /// <returns> 转化后的指定类型对象，转化失败时返回指定的默认值 </returns>
        public static T CastTo<T>(this object value, T defaultValue)
        {
            try
            {
                return To<T>(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
