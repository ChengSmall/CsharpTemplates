using System;
using System.Runtime.Remoting;

namespace Cheng.Memorys
{

    /// <summary>
    /// 一个高度封装的管理非托管资源安全释放的类
    /// </summary>
    /// <remarks>
    /// <para>派生该类的对象，无需从头编写<see cref="IDisposable"/>释放代码，只需重写<see cref="UnmanagedReleasources"/>方法或<see cref="Disposeing(bool)"/>，并添加资源释放代码即可；一般情况下无需重写<see cref="Close"/>方法，除非您要重新注释</para>
    /// <para>根据<see cref="IsNotDispose"/>和<see cref="IsDispose"/>属性判断对象是否已经释放相关资源</para>
    /// </remarks>
    public abstract class SafreleaseUnmanagedResources : IDisposable
    {

        /// <summary>
        /// 在派生类重写此方法，用于释放非托管资源
        /// </summary>
        /// <remarks>若需要根据条件清理托管资源和非托管资源，推荐重写<see cref="Disposeing(bool)"/></remarks>
        protected virtual void UnmanagedReleasources() { }

        /// <summary>
        /// 释放代码内终止了析构函数时调用该方法
        /// </summary>
        /// <remarks>
        /// <para>当调用清理函数<see cref="Dispose(bool)"/>参数为true，且<see cref="Disposeing(bool)"/>返回值为true时，会调用该方法一次；</para>
        /// </remarks>
        protected virtual void IsSuppressFunalize() { }

        /// <summary>
        /// 在派生类重写此方法，用于释放非托管资源和托管对象
        /// </summary>
        /// <remarks>该方法在首次调用<see cref="Dispose(bool)"/>方法时被调用，<paramref name="disposeing"/>参数由<see cref="Dispose(bool)"/>的参数传递</remarks>
        /// <param name="disposeing">是否清理托管资源对象</param>
        /// <returns>
        /// <para>是否关闭该对象的析构方法</para>
        /// <para>
        /// 返回false时，将不会对实例调用<see cref="GC.SuppressFinalize(object)"/>和<see cref="IsSuppressFunalize"/>；<br/>
        /// 返回true时，如果<see cref="Dispose(bool)"/>的参数为true，则会对实例调用<see cref="GC.SuppressFinalize(object)"/>和<see cref="IsSuppressFunalize"/>
        /// </para>
        /// <para>默认返回值为true</para>
        /// </returns>
        protected virtual bool Disposeing(bool disposeing) => true;

        /// <summary>
        /// 当前实例是否未释放非托管资源
        /// </summary>
        public bool IsNotDispose => !p_isDispose;

        /// <summary>
        /// 当前实例是否已被释放
        /// </summary>
        protected bool IsDispose => p_isDispose;

        #region 封装

        /// <summary>
        /// 构造
        /// </summary>
        protected SafreleaseUnmanagedResources()
        {
            p_isDispose = false;
        }

        private bool p_isDispose;

        /// <summary>
        /// 调用该方法清理非托管资源
        /// </summary>
        public virtual void Close()
        {
            Dispose(true);
        }

        /// <summary>
        /// 调用此方法清理非托管资源
        /// </summary>
        /// <param name="disposed">是否释放托管资源并停止析构方法；
        /// <para>参数为true时，在资源释放后若<see cref="Disposeing(bool)"/>的返回值为true，则会使用<see cref="GC.SuppressFinalize(object)"/>禁止该对象的对象终结器并调用<see cref="IsSuppressFunalize"/>；<br/>
        /// 若参数是false，则仅释放资源，且不会调用<see cref="IsSuppressFunalize"/>；一般在析构函数中调用时使用false</para>
        /// </param>
        /// <param name="notSuppressFinalize">如果该参数为true，则无论如何都不会使用<see cref="GC.SuppressFinalize(object)"/>来终止析构函数；参数为false则正常运行</param>
        protected void Dispose(bool disposed, bool notSuppressFinalize)
        {
            if (p_isDispose) return;
            p_isDispose = true;

            //----释放----
            UnmanagedReleasources();
            bool flag = Disposeing(disposed);
            //----释放----

            if (disposed && flag && (!notSuppressFinalize))
            {
                GC.SuppressFinalize(this);
                IsSuppressFunalize();
            }

        }

        /// <summary>
        /// 调用此方法清理非托管资源
        /// </summary>
        /// <param name="suppressFunalize">是否释放托管资源并停止析构方法；
        /// <para>参数为true时，在资源释放后若<see cref="Disposeing(bool)"/>的返回值为true，则会使用<see cref="GC.SuppressFinalize(object)"/>禁止该对象的对象终结器并调用<see cref="IsSuppressFunalize"/>；<br/>
        /// 若参数是false，则仅释放资源，且不会调用<see cref="IsSuppressFunalize"/>；一般在析构方法中调用时使用false</para>
        /// </param>
        protected void Dispose(bool suppressFunalize)
        {
            Dispose(suppressFunalize, false);

        }
        
        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 调用该函数，以此在实例资源已释放时引发<see cref="ObjectDisposedException"/>异常
        /// </summary>
        protected void ThrowObjectDisposeException()
        {
            if(p_isDispose) throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// 调用该函数，以此在实例资源已释放时引发<see cref="ObjectDisposedException"/>异常
        /// </summary>
        /// <param name="objName">引发异常的对象名称</param>
        protected void ThrowObjectDisposeException(string objName)
        {
            if (p_isDispose) throw new ObjectDisposedException(objName);
        }

        /// <summary>
        /// 调用该函数，以此在实例资源已释放时引发<see cref="ObjectDisposedException"/>异常
        /// </summary>
        /// <param name="objName">引发异常的对象名称</param>
        /// <param name="message">异常消息</param>
        protected void ThrowObjectDisposeException(string objName, string message)
        {
            if (p_isDispose) throw new ObjectDisposedException(objName, message);
        }

        #endregion

    }

}
