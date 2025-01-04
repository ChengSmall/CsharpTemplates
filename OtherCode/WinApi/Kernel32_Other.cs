using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Cheng.OtherCode.Winapi
{

    /// <summary>
    /// Kernel32库的其它api
    /// </summary>
    public unsafe static class Kernel32_Other
    {

        /// <summary>
        /// 检索调用线程的最后错误代码值
        /// </summary>
        /// <remarks>
        ///  最后一个错误代码按线程进行维护，多个线程不会覆盖彼此的最后一个错误代码
        /// </remarks>
        /// <returns>
        /// <para>返回值是调用线程的最后错误代码</para>
        /// <para>
        /// 设置最后错误代码的每个函数的文档的返回值部分记录了函数设置最后错误代码的条件；<br/>
        /// 设置线程最后错误代码的大多数函数在失败时设置它；但是，某些函数还会在成功时设置最后一个错误代码<br/>
        /// 如果未记录函数以设置最后一个错误代码，则此函数返回的值只是要设置的最新最后一个错误代码；某些函数在成功时将最后一个错误代码设置为0，而其他函数则不这样做
        /// </para>
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

    }
}
