using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Cheng.Memorys;

namespace Cheng.Windows.Processes
{

    /// <summary>
    /// 提供对进程间的内存读写操作
    /// </summary>
    /// <remarks>此功能仅对提供<see cref="WinAPI"/>的Windows操作系统启用</remarks>
    public sealed unsafe class ProcessOperation : ReleaseDestructor
    {

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            
            if (p_independentHandle)
            {
                //是独立句柄单独释放
                if(p_handle != null) WinAPI.CloseHandle(p_handle);
            }

            if (p_isFreeProcess && disposeing)
            {
                //释放托管实例
                p_process?.Close();
            }

            p_process = null;
            p_handle = null;

            return true;
        }

        /// <summary>
        /// 调用该方法释放进程句柄和相关资源
        /// </summary>
        public sealed override void Close()
        {
            Dispose(true);
        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化指定进程的操作实例
        /// </summary>
        /// <param name="processID">进程ID</param>
        /// <param name="access">进程访问权限枚举</param>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="InvalidOperationException">无法打开进程句柄</exception>
        /// <exception cref="NotSupportedException">无法访问句柄</exception>
        public ProcessOperation(int processID)
        {
            p_process = Process.GetProcessById(processID);
            p_handle = p_process.Handle.ToPointer();
            p_independentHandle = false;
            p_isFreeProcess = true;
        }

        /// <summary>
        /// 实例化进程操作实例
        /// </summary>
        /// <param name="process">封装进程对象</param>
        public ProcessOperation(Process process) : this(process, true)
        {
        }

        /// <summary>
        /// 实例化指定进程的操作实例，指定句柄权限
        /// </summary>
        /// <param name="processID">句柄ID</param>
        /// <param name="access">句柄操作权限</param>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="InvalidOperationException">无法打开进程句柄</exception>
        /// <exception cref="NotSupportedException">无法访问句柄</exception>
        public ProcessOperation(int processID, ProcessAccessFlags access)
        {
            p_handle = WinAPI.OpenProcess((uint)access, false, processID);
            if (p_handle == null)
            {
                throw new NotSupportedException();
            }

            p_process = Process.GetProcessById(processID);

            p_independentHandle = true;
            p_isFreeProcess = true;
        }

        /// <summary>
        /// 实例化进程操作实例
        /// </summary>
        /// <param name="process">封装进程对象</param>
        /// <param name="releaseProcess">在释放资源时是否将内部<paramref name="process"/>释放；默认为true</param>
        public ProcessOperation(Process process, bool releaseProcess)
        {
            if (process is null) throw new ArgumentNullException();

            p_process = process;
            p_handle = p_process.Handle.ToPointer();
            p_independentHandle = false;
            p_isFreeProcess = releaseProcess;

        }

        #endregion

        #region 参数

        private Process p_process;
        private void* p_handle;

        /// <summary>
        /// 是独立句柄
        /// </summary>
        private bool p_independentHandle;
        /// <summary>
        /// 是否释放Process
        /// </summary>
        private bool p_isFreeProcess;
        #endregion

        #region 功能

        #region 封装


        #endregion

        #region 参数访问

        /// <summary>
        /// 获取该实例关联的进程ID
        /// </summary>
        /// <exception cref="InvalidOperationException">错误</exception>
        /// <exception cref="PlatformNotSupportedException">平台错误</exception>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        public int Id
        {
            get
            {
                ThrowObjectDisposeException();
                if(p_independentHandle)
                {
                    uint id;
                    id = WinAPI.GetProcessId(p_handle);
                    if (id == 0) return p_process.Id;

                    return (int)id;
                }
                return p_process.Id;
            }
        }

        /// <summary>
        /// 获取关联此实例进程的主模块
        /// </summary>
        /// <exception cref="Exception">错误</exception>
        public ProcessModule MainModule
        {
            get
            {
                ThrowObjectDisposeException();
                return p_process.MainModule;
            }
        }

        /// <summary>
        /// 获取关联此实例进程的所有模块
        /// </summary>
        /// <exception cref="Exception">错误</exception>
        public ProcessModuleCollection Modules
        {
            get
            {
                ThrowObjectDisposeException();
                return p_process.Modules;
            }
        }

        /// <summary>
        /// 获取进程关联实例
        /// </summary>
        /// <returns>若实例已释放，则返回null</returns>
        public Process Process
        {
            get
            {
                return p_process;
            }
        }

        /// <summary>
        /// 判断该实例进程是否在Wow64运行环境中
        /// </summary>
        /// <remarks>
        /// 如果是处于32位操作系统下，返回false；如果是64位进程，返回false；只有在64位操作系统下运行32位程序时，返回true；
        /// <para>关于Wow64，详见 https://learn.microsoft.com/windows/win32/winprog64/running-32-bit-applications </para>
        /// </remarks>
        /// <exception cref="Win32Exception">错误</exception>
        public bool IsWow64
        {
            get
            {
                int re;
                if(WinAPI.IsWow64Process(p_handle, &re)) return re != 0;

                throw new Win32Exception();
            }
        }

        /// <summary>
        /// 当前活动的进程是否为64位运行环境
        /// </summary>
        public static bool X64
        {
            get => sizeof(void*) == 8;
        }

        #endregion

        #region 读写

        /// <summary>
        /// 读取该进程中指定地址的数据
        /// </summary>
        /// <param name="readProcessAddress">要读取的进程内地址</param>
        /// <param name="readBuffer">要将数据读取到的内存</param>
        /// <param name="readSize">要读取的字节大小</param>
        /// <param name="realReadSize">返回实际读取的字节大小</param>
        /// <returns>是否成功读取，成功返回true，否则返回false</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public bool Read(IntPtr readProcessAddress, IntPtr readBuffer, int readSize, out IntPtr realReadSize)
        {
            ThrowObjectDisposeException();
            if (readSize < 0) throw new ArgumentOutOfRangeException();
            if (readProcessAddress == IntPtr.Zero || readBuffer == IntPtr.Zero) throw new ArgumentNullException();

            IntPtr pr;
            bool flag = WinAPI.ReadProcessMemory(p_handle, readProcessAddress.ToPointer(),
                readBuffer.ToPointer(),
                (uint)readSize, &pr);
            realReadSize = pr;
            return flag;
        }

        /// <summary>
        /// 读取该进程中指定地址的数据
        /// </summary>
        /// <param name="readProcessAddress">要读取的进程内地址</param>
        /// <param name="readBuffer">要将数据读取到的内存</param>
        /// <param name="readSize">要读取的字节大小</param>
        /// <returns>是否成功读取，成功返回true，否则返回false</returns>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public bool Read(IntPtr readProcessAddress, IntPtr readBuffer, int readSize)
        {
            ThrowObjectDisposeException();
            if (readSize < 0) throw new ArgumentOutOfRangeException();
            if (readProcessAddress == IntPtr.Zero || readBuffer == IntPtr.Zero) throw new ArgumentNullException();

            return WinAPI.ReadProcessMemory(p_handle, readProcessAddress.ToPointer(), readBuffer.ToPointer(),
                (uint)readSize, null);
        }

        /// <summary>
        /// 在该进程指定地址位置改写数据
        /// </summary>
        /// <param name="writeProcessAddress">要改写的进程内地址</param>
        /// <param name="writeBuffer">待写入的数据所在内存</param>
        /// <param name="writeSize">要写入的字节大小</param>
        /// <param name="realWriteSize">返回实际写入的字节大小</param>
        /// <returns>是否成功写入，成功返回true，否则返回false</returns>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public bool Write(IntPtr writeProcessAddress, IntPtr writeBuffer, int writeSize, out IntPtr realWriteSize)
        {
            ThrowObjectDisposeException();
            if (writeSize < 0) throw new ArgumentOutOfRangeException();
            if (writeProcessAddress == IntPtr.Zero || writeBuffer == IntPtr.Zero) throw new ArgumentNullException();

            IntPtr pr;
            bool flag = WinAPI.WriteProcessMemory(p_handle, writeProcessAddress.ToPointer()
                , writeBuffer.ToPointer(),
                (uint)writeSize, &pr);
            realWriteSize = pr;
            return flag;
        }

        /// <summary>
        /// 在该进程指定地址位置改写数据
        /// </summary>
        /// <param name="writeProcessAddress">要改写的进程内地址</param>
        /// <param name="writeBuffer">待写入的数据所在内存</param>
        /// <param name="writeSize">要写入的字节大小</param>
        /// <returns>是否成功写入，成功返回true，否则返回false</returns>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public bool Write(IntPtr writeProcessAddress, IntPtr writeBuffer, int writeSize)
        {
            ThrowObjectDisposeException();
            if (writeSize < 0) throw new ArgumentOutOfRangeException();
            if (writeProcessAddress == IntPtr.Zero || writeBuffer == IntPtr.Zero) throw new ArgumentNullException();

            return WinAPI.WriteProcessMemory(p_handle, writeProcessAddress.ToPointer(), 
                writeBuffer.ToPointer(),
                (uint)writeSize, null);
        }

        /// <summary>
        /// 读取该进程内存到指定变量
        /// </summary>
        /// <typeparam name="T">读取的变量类型</typeparam>
        /// <param name="processAddress">要读取的进程首地址</param>
        /// <param name="value">要将进程内存读取到的变量</param>
        /// <returns>是否成功读取；成功读取返回true，读取失败或读取的字节数量不完整返回false</returns>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public bool Read<T>(IntPtr processAddress, out T value) where T : unmanaged
        {
            ThrowObjectDisposeException();
            fixed (T* tp = &value)
            {
                *tp = default;
                ulong rsize;
                bool flag = WinAPI.ReadProcessMemory(p_handle, processAddress.ToPointer(), tp, (uint)sizeof(T), &rsize);

                return flag && (rsize == (ulong)sizeof(T));
            }
        }

        /// <summary>
        /// 将指定变量内存写入到进程
        /// </summary>
        /// <typeparam name="T">写入的变量类型</typeparam>
        /// <param name="processAddress">要写入的进程首地址</param>
        /// <param name="value">要写入的变量</param>
        /// <returns>是否成功写入</returns>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public bool Write<T>(IntPtr processAddress, T value) where T : unmanaged
        {
            ThrowObjectDisposeException();
            ulong rsize;
            bool flag = WinAPI.WriteProcessMemory(p_handle, processAddress.ToPointer(), &value, (uint)sizeof(T), &rsize);
            return flag && (rsize == (ulong)sizeof(T));
        }

        /// <summary>
        /// 将指定内存写入进程
        /// </summary>
        /// <param name="processAddress">要写入到的进程所在地址</param>
        /// <param name="buffer">表示待写入数据的字节数组</param>
        /// <param name="offset">从缓冲区所在的起始位置开始写入</param>
        /// <param name="count">写入的字节大小</param>
        /// <returns>实际写入的字节数；若无法写入或有其它错误，则返回-1</returns>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentException">参数错误</exception>
        public int Write(IntPtr processAddress, byte[] buffer, int offset, int count)
        {
            ThrowObjectDisposeException();
            if (buffer is null || processAddress == IntPtr.Zero) throw new ArgumentNullException();

            if (offset < 0 || count < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException();

            void* re = default;
            fixed (byte* bufptr = buffer)
            {
                bool flag = WinAPI.WriteProcessMemory(p_handle, processAddress.ToPointer(), bufptr + offset, (uint)count, &re);
                if (!flag) return -1;
            }
            return (int)re;
        }

        /// <summary>
        /// 将进程指定地址读取到缓冲区
        /// </summary>
        /// <param name="processAddress">到读取的进程地址</param>
        /// <param name="buffer">要将数据读取到的缓冲区</param>
        /// <param name="offset">缓冲区接收数据时的起始位置</param>
        /// <param name="count">想要读取的字节数量</param>
        /// <returns>实际读取的字节数；若无法写入或有其它错误，则返回-1</returns>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        /// <exception cref="ObjectDisposedException">资源已释放</exception>
        public int Read(IntPtr processAddress, byte[] buffer, int offset, int count)
        {
            ThrowObjectDisposeException();
            if (buffer is null || processAddress == IntPtr.Zero) throw new ArgumentNullException();


            if (offset < 0 || count < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException();

            IntPtr re = default;
            fixed (byte* bufptr = buffer)
            {
                bool flag = WinAPI.ReadProcessMemory(p_handle, processAddress.ToPointer(), bufptr + offset, (uint)count, &re);
                if (!flag) return -1;
            }

            return (int)re.ToPointer();
        }

        #endregion

        #region 进程句柄创建

        /// <summary>
        /// 创建指定名称的进程操作实例
        /// </summary>
        /// <param name="name">进程名称（主模块名称）</param>
        /// <returns>新创建的指定名称进程操作实例；若没有找到指定名称的进程，返回null</returns>
        /// <exception cref="Exception">错误</exception>
        public static ProcessOperation GetProcessByName(string name)
        {

            var pros = Process.GetProcesses();

            int length = pros.Length;
            int i;
            ProcessModule pm;
            int id = -1;
            for (i = 0; i < length; i++)
            {
                try
                {
                    pm = pros[i].MainModule;
                    if (pm.ModuleName == name && id == -1) id = pros[i].Id;
                }
                catch (Exception)
                {
                }
               
                pros[i].Close();
            }

            if (id == -1) return null;

            return new ProcessOperation(id);

        }

        /// <summary>
        /// 创建指定名称的进程操作实例
        /// </summary>
        /// <param name="name">进程名称（主模块名称）</param>
        /// <param name="access">进程操作权限</param>
        /// <returns>新创建的指定名称进程操作实例；若没有找到指定名称的进程，返回null</returns>
        /// <exception cref="Exception">错误</exception>
        public static ProcessOperation GetProcessByName(string name, ProcessAccessFlags access)
        {

            var pros = Process.GetProcesses();

            int length = pros.Length;
            int i;
            ProcessModule pm;
            int id = -1;
            for (i = 0; i < length; i++)
            {
                try
                {
                    pm = pros[i].MainModule;
                    if (pm.ModuleName == name && id == -1) id = pros[i].Id;
                }
                catch (Exception)
                {
                }

                pros[i].Close();
            }

            if (id == -1) return null;

            return new ProcessOperation(id, access);
        }

        #endregion

        #endregion

    }

}
