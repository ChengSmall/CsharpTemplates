using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Cheng.Consoles
{

    /// <summary>
    /// 控制台虚拟终端
    /// </summary>
    [Obsolete("API移至ConsoleSystem", true)]
    public static class VirtualTerminalConsole
    {

        [Obsolete("", true)]
        public static uint TryEnableVirtualTerminalProcessingOnWindows()
        {
            return ConsoleSystem.TryEnableVirtualTerminalProcessingOnWindows();
        }

        [Obsolete("", true)]
        public static void EnableVirtualTerminalProcessingOnWindows()
        {
            ConsoleSystem.EnableVirtualTerminalProcessingOnWindows();
        }

        [Obsolete("", true)]
        public static uint TryDisableVirtualTerminalProcessingOnWindows()
        {
            return ConsoleSystem.TryDisableVirtualTerminalProcessingOnWindows();
        }

        [Obsolete("", true)]
        public static void DisableVirtualTerminalProcessingOnWindows()
        {
            ConsoleSystem.DisableVirtualTerminalProcessingOnWindows();
        }

    }

}
