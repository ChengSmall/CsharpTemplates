using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Cheng.Memorys
{
    
    /// <summary>
    /// 安全管理非托管内存的内存分配器
    /// </summary>
    public sealed unsafe class UnmanagedMemoryagement : ReleaseDestructor
    {

        #region 结构

        #endregion

        #region 构造

        /// <summary>
        /// 实例化一个内存分配器
        /// </summary>
        public UnmanagedMemoryagement()
        {
            p_list = new Dictionary<IntPtr, int>();
            p_byteSize = 0;
        }

        #endregion

        #region 参数

        private long p_byteSize;
        private Dictionary<IntPtr, int> p_list;        

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                lock (p_list)
                {
                    foreach (var item in p_list)
                    {
                        f_freeFunc(item.Key);
                    }
                    p_list.Clear();
                    p_byteSize = -1;
                }
            }
            else
            {
                foreach (var item in p_list)
                {
                    f_freeFunc(item.Key);
                }
                //p_list.Clear();
                //p_byteSize = -1;
            }

            return true;
        }

        static void f_freeFunc(IntPtr m)
        {
            Marshal.FreeHGlobal(m);
        }

        #endregion

        #region 参数访问

        /// <summary>
        /// 当前实例的可用内存总字节量
        /// </summary>
        /// <returns>若已释放则返回-1</returns>
        public long AllSize
        {
            get
            {
                return p_byteSize;
            }
        }

        #endregion

        #region 内存管理

        /// <summary>
        /// 获取已分配内存的大小
        /// </summary>
        /// <param name="ptr">要获取的内存地址</param>
        /// <returns>内存的字节大小；如果不是当前实例管理的内存或内存已释放则返回-1</returns>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        public int GetMemorySize(IntPtr ptr)
        {
            ThrowObjectDisposeException();
            lock (p_list)
            {
                if (p_list.TryGetValue(ptr, out var size))
                {
                    return size;
                }
            }           
            return -1;
        }

        /// <summary>
        /// 开辟指定字节大小的非托管内存
        /// </summary>
        /// <param name="cb">指定字节大小</param>
        /// <returns>指向新分配的内存指针</returns>
        /// <exception cref="ArgumentOutOfRangeException">指定字节大小不能小于或等于0</exception>
        /// <exception cref="OutOfMemoryException">内存空间不足</exception>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        public IntPtr AllocMemory(int cb)
        {
            ThrowObjectDisposeException();
            if (cb <= 0) throw new ArgumentOutOfRangeException();
            lock (p_list)
            {               
                var ptr = Marshal.AllocHGlobal(cb);
                p_list.Add(ptr, cb);
                p_byteSize += cb;
                return ptr;
            }
        }

        /// <summary>
        /// 从非托管内存中释放已分配的内存
        /// </summary>
        /// <param name="ptr">要释放的内存指针</param>
        /// <returns>是否成功释放，成功释放返回true；如果指定内存不是已分配内存返回false</returns>
        public bool FreeMemory(IntPtr ptr)
        {
            ThrowObjectDisposeException();
            lock (p_list)
            {
                if (ptr == IntPtr.Zero) return false;

                var b = p_list.TryGetValue(ptr, out int size);
                if (!b) return false;
                p_list.Remove(ptr);

                f_freeFunc(ptr);
                p_byteSize -= size;
                return true;
            }
        }

        /// <summary>
        /// 判断指定地址是否已被分配内存并被该实例管理
        /// </summary>
        /// <param name="ptr">要判断的内存</param>
        /// <returns>true表示内存<paramref name="ptr"/>已被分配，false则表示没有分配</returns>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        public bool IsAllocMemory(IntPtr ptr)
        {
            ThrowObjectDisposeException();
            lock (p_list) return p_list.ContainsKey(ptr);
        }

        /// <summary>
        /// 释放当前已分配的所有内存
        /// </summary>
        /// <exception cref="ObjectDisposedException">实例已释放</exception>
        public void FreeAllMemory()
        {
            ThrowObjectDisposeException();
            lock (p_list)
            {
                foreach (var item in p_list)
                {
                    f_freeFunc(item.Key);
                }
                p_list.Clear();
                p_byteSize = 0;
            }
        }

        /// <summary>
        /// 开辟指定类型大小的非托管内存并返回值引用
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="memory">内存所在地址</param>
        /// <returns>值引用</returns>
        public ref T AllocStructure<T>(out IntPtr memory) where T : unmanaged
        {
            memory = AllocMemory(sizeof(T));
            return ref *(T*)memory;
        }

        /// <summary>
        /// 从指定值引用中释放已分配的内存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="refValue">值引用</param>
        /// <returns>是否成功释放，成功释放返回true；如果指定内存不是已分配内存返回false</returns>
        public bool FreeStructure<T>(ref T refValue) where T : unmanaged
        {
            fixed (T* ptr = &refValue)
            {
                IntPtr ip = new IntPtr(ptr);
                return FreeMemory(ip);
            }
           
        }

        /// <summary>
        /// 获取指定地址和给定类型偏移处的值
        /// </summary>
        /// <remarks>
        /// 只有在返回值为true，<paramref name="offsetOutAddress"/>为false时，该功能才会成功执行
        /// </remarks>
        /// <typeparam name="T">非托管变量类型</typeparam>
        /// <param name="address">地址</param>
        /// <param name="offset">
        /// 类型偏移值，最小单位为类型大小
        /// <para>例：如果 <typeparamref name="T"/> 是 <see cref="int"/> 时，字节偏移为 <paramref name="offset"/> * 4</para>
        /// </param>
        /// <param name="value">要获取的值引用</param>
        /// <param name="offsetOutAddress">给定的最终目标地址是否超出<paramref name="address"/>的可用内存范围</param>
        /// <returns>给定的<paramref name="address"/>是否处于当前对象管理范围内</returns>
        public bool GetValueOffset<T>(IntPtr address, int offset, out T value, out bool offsetOutAddress) where T : unmanaged
        {

            lock (p_list)
            {
                int size;
                value = default;
                offsetOutAddress = false;

                if (!p_list.TryGetValue(address, out size))
                {
                    //不在
                    return false;
                }
                
                if ((offset * sizeof(T)) + sizeof(T) > size)
                {
                    //超出可用地址
                    offsetOutAddress = true;
                    return true;
                }

                T* ptr = (T*)address;
                ptr += offset;

                value = *ptr;

                return true;

            }

        }

        /// <summary>
        /// 设置指定地址和给定类型偏移处的值
        /// </summary>
        /// <remarks>只有在返回值为true，<paramref name="offsetOutAddress"/>为false时，该功能才会成功执行</remarks>
        /// <typeparam name="T">非托管变量类型</typeparam>
        /// <param name="address">地址</param>
        /// <param name="offset">
        /// 类型偏移值，最小单位为类型大小
        /// <para>例：如果 <typeparamref name="T"/> 是 <see cref="int"/> 时，字节偏移为 <paramref name="offset"/> * 4</para>
        /// </param>
        /// <param name="value">要设置的值</param>
        /// <param name="offsetOutAddress">给定的最终目标地址是否超出<paramref name="address"/>的可用内存范围</param>
        /// <returns>给定的<paramref name="address"/>是否处于当前对象管理范围内</returns>
        public bool SetValueOffset<T>(IntPtr address, int offset, T value, out bool offsetOutAddress) where T : unmanaged
        {

            lock (p_list)
            {
                int size;

                offsetOutAddress = false;

                if (!p_list.TryGetValue(address, out size))
                {
                    //不在
                    return false;
                }

                if ((offset * sizeof(T)) + sizeof(T) > size)
                {
                    //超出可用地址
                    offsetOutAddress = true;
                    return true;
                }

                T* ptr = (T*)address;
                ptr += offset;

                *ptr = value;
                return true;

            }
        }


        #endregion

    }

}
