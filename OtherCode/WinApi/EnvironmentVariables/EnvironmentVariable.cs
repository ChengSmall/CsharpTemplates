using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace Cheng.OtherCode.Winapi
{


    public static unsafe class EnvironmentVariable
    {

        private const string dll = "kernel32.dll";

        /// <summary>
        /// 设置当前进程的指定环境变量的内容
        /// </summary>
        /// <param name="lpName">
        /// <para>环境变量的名称</para>
        /// <para>如果环境变量不存在且 <paramref name="lpValue"/> 不为 null，则操作系统将创建该环境变量</para>
        /// </param>
        /// <param name="lpValue">
        /// <para>要设置的环境变量内容，如果是null表示删除</para>
        /// <para>用户定义的环境变量的最大大小为 32767 个字符</para>
        /// </param>
        /// <returns>如果该函数成功，则返回值为true，失败则返回false</returns>
        [DllImport(dll, BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint SetEnvironmentVariable(string lpName, string lpValue);

        /// <summary>
        /// 从调用进程的环境块检索指定变量的内容
        /// </summary>
        /// <remarks>
        /// <para>此函数可以检索系统环境变量或用户环境变量</para>
        /// </remarks>
        /// <param name="lpName">环境变量的名称</param>
        /// <param name="lpBuffer">
        /// <para>指向缓冲区的指针</para>
        /// <para>该缓冲区以 '\0' 结尾的字符串的形式接收指定环境变量的内容；环境变量的最大大小限制为包括 '\0' 终止字符在内的 32767 个字符</para>
        /// </param>
        /// <param name="size">缓冲区的实际可用容量大小，以字符数为单位</param>
        /// <returns>
        /// <para>如果函数成功，则返回<paramref name="lpBuffer"/>指向的缓冲区中存储的字符数，不包括 '\0' 终止字符</para>
        /// <para>如果<paramref name="lpBuffer"/>的容量不够足以写入数据，则返回值是需写入字符串及其 '\0' 终止字符所需的缓冲区大小（以字符为单位），此时<paramref name="lpBuffer"/>的内容无效</para>
        /// <para>如果函数失败，则返回0；如果在环境块中找不到指定的环境变量， GetLastError 将返回 ERROR_ENVVAR_NOT_FOUND (203)</para>
        /// </returns>
        [DllImport(dll, BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetEnvironmentVariable(string lpName, char* lpBuffer, int size);

        /// <summary>
        /// 检索当前进程的环境变量
        /// </summary>
        /// <remarks>
        /// <para>函数返回指向内存块的指针，该内存块包含调用进程的环境变量 (系统和用户环境变量)</para>
        /// <para>
        /// 每个环境块包含以下格式的环境变量<br/>
        /// <code>
        /// name1=value1\0
        /// name2=value2\0
        /// name3=value3\0
        /// ...
        /// nameN=valueN\0\0
        /// </code>
        /// 每个name=value字符串都使用一个\0作为结尾，而在整个环境快字符串末尾，还有一个\0；<br/>
        /// 环境变量的名称不能包含等号
        /// </para>
        /// </remarks>
        /// <returns>
        /// <para>如果函数成功，则返回值是指向当前进程的环境块的指针；失败则为null</para>
        /// <para>不再使用后，需要调用<see cref="FreeEnvironmentStrings(char*)"/>释放内存</para>
        /// </returns>
        [DllImport(dll, CharSet = CharSet.Unicode)]
        public unsafe static extern char* GetEnvironmentStrings();

        /// <summary>
        /// 释放环境字符串块
        /// </summary>
        /// <param name="pStrings">指向环境字符串块的指针</param>
        /// <returns>如果函数成功，则返回值为非零，失败返回0</returns>
        [DllImport(dll, CharSet = CharSet.Unicode, SetLastError = true)]
        public unsafe static extern uint FreeEnvironmentStrings(char* pStrings);

    }

}