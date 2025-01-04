using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Cheng.OtherCode.TESTS
{
    
    public static class ProcessStart
    {

        /// <summary>
        /// 将进程以管理员模式启动时需要的参数
        /// </summary>
        /// <param name="info"></param>
        public static void ProcessAdministratorStart(ProcessStartInfo info)
        {
            info.Verb = "runas";
        }

    }

}
