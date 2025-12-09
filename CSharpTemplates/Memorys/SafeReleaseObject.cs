using System;
using System.Runtime.InteropServices;

namespace Cheng.Memorys
{

    /// <summary>
    /// 安全释放非托管对象封装
    /// </summary>
    /// <remarks>
    /// <para>封装一个非托管资源的对象，以保证在托管对象释放前一定会释放非托管资源</para>
    /// <para>适用于必须释放非托管资源，却没有实现对象终结器（析构函数）的类型</para>
    /// <para>
    /// 某些第三方库，在需要释放非托管资源的类中实现了<see cref="IDisposable"/>，却没有实现对象终结器（析构函数）；该类帮助这些不完整的实现达到绝对安全的内存管理；
    /// </para>
    /// <para>注意不要封装已经实现安全释放和对象终结器（析构函数）的实例，有可能会导致在对象终结器（析构函数）线程内释放托管对象</para>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public sealed class SafeReleaseObject<T> : SafreleaseUnmanagedResources where T : class, System.IDisposable
    {

        #region 构造

        /// <summary>
        /// 实例化一个安全释放非托管对象封装
        /// </summary>
        /// <param name="obj">
        /// 要封装的非托管资源对象
        /// </param>
        public SafeReleaseObject(T obj)
        {
            if (obj is null) throw new ArgumentNullException();
            p_obj = obj;
        }

        #endregion

        #region 释放

        protected sealed override void UnmanagedReleasources()
        {
            p_obj.Dispose();
        }

        /// <summary>
        /// 保证在该托管对象释放前调用非托管资源释放器
        /// </summary>
        ~SafeReleaseObject()
        {
            Dispose(false);
        }

        #endregion

        #region 参数

        private T p_obj;

        #endregion

        #region 派生

        /// <summary>
        /// 获取内部封装的非托管对象
        /// </summary>
        public T DisposingObject => p_obj;

        #endregion

    }

    /// <summary>
    /// 使用<see cref="GCHandle"/>将托管对象固定为一个无法被GC回收的对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed unsafe class SafeAllocObject<T> : ReleaseDestructor where T : class
    {

        #region 参数

        /// <summary>
        /// 实例化一个固定对象
        /// </summary>
        /// <param name="obj"></param>
        public SafeAllocObject(T obj)
        {
            p_gc = GCHandle.Alloc(obj, GCHandleType.Pinned);
            try
            {
                p_obj = (p_gc.Target as T) ?? throw new ArgumentNullException();
            }
            catch (Exception)
            {
                if(p_gc.IsAllocated) p_gc.Free();
                throw;
            }
        }

        private GCHandle p_gc;
        private T p_obj;

        #endregion

        #region 释放

        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                p_gc.Free();
            }
            p_obj = null;
            return true;
        }

        #endregion

        #region 功能

        /// <summary>
        /// 获取已固定的对象
        /// </summary>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public T Target
        {
            get
            {
                ThrowObjectDisposeException();
                return p_obj;
            }
        }

        /// <summary>
        /// 获取对象固定后的地址
        /// </summary>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        public IntPtr IntPointer
        {
            get
            {
                ThrowObjectDisposeException();
                return p_gc.AddrOfPinnedObject();
            }
        }

        /// <summary>
        /// 获取对象固定后的地址，如果对象已释放返回null
        /// </summary>
        public void* Pointer
        {
            get
            {
                ThrowObjectDisposeException();
                if (IsDispose) return null;
                return p_gc.AddrOfPinnedObject().ToPointer();
            }
        }

        #endregion

    }

}
