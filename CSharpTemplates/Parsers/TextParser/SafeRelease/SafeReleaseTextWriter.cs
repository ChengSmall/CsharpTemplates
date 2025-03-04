using System;
using System.IO;

namespace Cheng.Texts
{

    /// <summary>
    /// 安全释放封装的文本写入器
    /// </summary>
    public abstract class SafeReleaseTextWriter : TextWriter
    {

        #region 封装

        private bool p_isDispose = false;

        /// <summary>
        /// 关闭该实例并释放关联的资源
        /// </summary>
        public override void Close()
        {
            Dispose(true, false);
        }

        /// <summary>
        /// 调用该方法释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管实例并终止对象终结器</param>
        protected sealed override void Dispose(bool disposing)
        {
            Dispose(disposing, false);
        }

        /// <summary>
        /// 调用此方法清理非托管资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源并停止析构方法；
        /// <para>参数为true时，在资源释放后若<see cref="Disposing(bool)"/>的返回值为true，则会使用<see cref="GC.SuppressFinalize(object)"/>禁止该对象的对象终结器并调用<see cref="IsSuppressFinalize"/>；<br/>
        /// 若参数是false，则仅释放资源，且不会调用<see cref="IsSuppressFinalize"/>；一般在析构函数中调用时使用false</para>
        /// </param>
        /// <param name="notSuppressFinalize">如果该参数为true，则无论如何都不会使用<see cref="GC.SuppressFinalize(object)"/>来终止析构函数；参数为false则正常运行</param>
        protected void Dispose(bool disposing, bool notSuppressFinalize)
        {
            if (p_isDispose) return;
            p_isDispose = true;

            //----释放----
            ReleaseResources();
            bool flag = Disposing(disposing);
            //----释放----

            if (disposing && flag && (!notSuppressFinalize))
            {
                GC.SuppressFinalize(this);
                IsSuppressFinalize();
            }
        }

        #endregion

        #region api

        /// <summary>
        /// 重写该方法以释放非托管资源
        /// </summary>
        /// <remarks>该方法会在资源释放时调用一次</remarks>
        protected virtual void ReleaseResources()
        {
        }

        /// <summary>
        /// 该方法会在释放资源的同时停止对象终结器（析构函数）时调用一次
        /// </summary>
        protected virtual void IsSuppressFinalize() { }

        /// <summary>
        /// 重写该方法以释放非托管资源和托管资源
        /// </summary>
        /// <param name="disposeing">是否释放托管资源；该参数是调用<see cref="Dispose(bool)"/>时的参数</param>
        /// <returns>true表示释放后停止对象终结器（析构函数），false则不停止对象终结器（析构函数）；默认返回true</returns>
        protected virtual bool Disposing(bool disposeing)
        {
            return true;
        }

        /// <summary>
        /// 该实例是否已被释放
        /// </summary>
        public bool IsDispose
        {
            get => p_isDispose;
        }

        /// <summary>
        /// 该方法会判断实例是否释放，若释放则引发<see cref="ObjectDisposedException"/>
        /// </summary>
        protected void ThrowObjectDisposed()
        {
            if (p_isDispose) throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

    }

}
