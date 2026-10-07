using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Cheng.DataStructure;
using Cheng.Algorithm;
using Cheng.Memorys;
using Cheng.DataStructure.Windows;

namespace Cheng.Systems
{

    /// <summary>
    /// win32 .Net Framework 系统
    /// </summary>
    public static unsafe partial class SystemEnvironmentFramework
    {

        #region 权限

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
                return ProcessUser(WindowsBuiltInRole.Administrator);
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
                    flag = new WindowsPrincipal(identity).IsInRole(role);
                }
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        #endregion

    }

}
#if DEBUG
#endif