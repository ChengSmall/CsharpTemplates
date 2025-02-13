using System;
using System.Diagnostics;
using System.Security.Principal;

namespace Cheng.Systems
{

    /// <summary>
    /// 系统环境参数
    /// </summary>
    public unsafe static class SystemEnvironment
    {

        #region 环境判断

        /// <summary>
        /// 判断此进程是否为管理员权限
        /// </summary>
        /// <returns>
        /// 是管理员权限返回true，否则返回false
        /// <para>获取此属性值会在内部申请非托管对象并销毁，因此最好不要频繁调用</para>
        /// </returns>
        public static bool IsAdministrator
        {
            get
            {
                bool flag;
                try
                {
                    using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                    {
                        var ws = new WindowsPrincipal(identity);
   
                        flag = ws.IsInRole(WindowsBuiltInRole.Administrator);
                    }
                }
                catch (Exception)
                {
                    flag = false;
                }
                return flag;
            }
        }

        /// <summary>
        /// 判断此进程是否为指定的用户权限
        /// </summary>
        /// <param name="role">用户权限</param>
        /// <returns>
        /// 是指定的用户权限返回true，否则返回false
        /// <para>获取此属性值会在内部申请非托管对象并销毁，因此最好不要频繁调用</para>
        /// </returns>
        public static bool ProcessUser(WindowsBuiltInRole role)
        {
            bool flag;
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    // 使用身份对象进行操作
                    var ws = new WindowsPrincipal(identity);

                    flag = ws.IsInRole(role);
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断此进程为64位运行环境
        /// </summary>
        /// <returns>若此进程是64位运行环境返回true，否则返回false</returns>
        public static bool X64
        {
            get => sizeof(void*) == 8;
        }

        /// <summary>
        /// 获取当前进程的线程数量
        /// </summary>
        /// <returns>当前进程的线程数量，此属性会创建进程资源再销毁，因此最好不要频繁调用</returns>
        /// <exception cref="PlatformNotSupportedException"></exception>
        /// <exception cref="SystemException"></exception>
        public static int ThreadCount
        {
            get
            {
                int count;
                Process pro = null;
                try
                {

                    pro = Process.GetCurrentProcess();

                    count = pro.Threads.Count;

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    pro?.Close();
                }

                return count;
            }
        }

        #endregion

        #region 命令行

        /// <summary>
        /// 获取一个命令行参数，若没有命令行参数则为空数组
        /// </summary>
        /// <exception cref="NotSupportedException">系统不支持命令行参数</exception>
        /// <returns>一个表示命令行参数的字符串数组实例，返回的对象是新申请的实例</returns>
        public static string[] GetCommandArgs()
        {
            string[] s = Environment.GetCommandLineArgs();
            int length = s.Length - 1;
            string[] args = new string[length];
            Array.Copy(s, 1, args, 0, length);
            return args;
        }

        /// <summary>
        /// 获取一个命令行参数，若没有命令行参数则为空数组
        /// </summary>
        /// <param name="args">获取的表示命令行参数的字符串数组实例，若无法成功获取则为null，返回的对象是新申请的实例</param>
        /// <returns>是否成功获取，成功获取返回true，否则返回false</returns>
        public static bool TryGetCommandArgs(out string[] args)
        {
            try
            {
                args = GetCommandArgs();
                return true;
            }
            catch (Exception)
            {
                args = null;
                return false;
            }
        }

        #endregion

    }

}
