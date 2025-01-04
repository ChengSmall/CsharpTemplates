using System;

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

}
