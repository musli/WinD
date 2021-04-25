using System;
using System.Collections.Generic;
using System.Text;

namespace WinD.Plug.SystemMonitoring
{
    public class DeskHelper
    {
        private static Random Random = new Random();
        public static int GetDeskUseRate()
        {
            return Random.Next(0, 100);
        }
    }
}
