using System;
using System.Runtime.InteropServices;

namespace Cheng.Windows.Win32API
{


    public static class ErrorCodeAPI
    {


        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();


        [DllImport("kernel32.dll")]
        public static extern void SetLastError(uint error);


    }


}
