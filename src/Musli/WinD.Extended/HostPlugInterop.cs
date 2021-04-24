using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinD.Extended
{
    /// <summary>
    /// 宿主插件交互类
    /// </summary>
    public class HostPlugInterop
    {
        /// <summary>
        /// 宿主句柄
        /// </summary>
        public static IntPtr HostHwnd = IntPtr.Zero;
        /// <summary>
        /// 宿主的某一属性改变时发生
        /// </summary>
        public static event Action<string, object> HostChanged;

        /// <summary>
        /// 宿主属性变更时触发
        /// </summary>
        /// <param name="command">
        /// 命令如下
        /// "WidthChanged" double
        /// "OpacityChanged" double
        /// </param>
        /// <param name="para">参数</param>
        public static void OnHostChanged(string command, object para)
        {
            HostChanged?.Invoke(command, para);
        }
    }

}
