using System;
using System.IO;

namespace Cheng.Streams
{

    /// <summary>
    /// 释放方法高度封装的流
    /// </summary>
    public unsafe abstract class HEStream : Stream
    {

        #region 释放

        /// <summary>
        /// 此实例的资源是否被释放
        /// </summary>
        public bool IsDispose
        {
            get => p_isDispose;
        }

        /// <summary>
        /// 重写此方法以释放非托管资源
        /// </summary>
        /// <remarks>该方法只会在释放资源时执行一次，用于清理或释放所有非托管资源</remarks>
        protected virtual void DisposeUnmanaged() { }

        /// <summary>
        /// 重写此函数释放资源
        /// </summary>
        /// <param name="disposing">是否清理托管资源</param>
        /// <returns>是否仍继续调用<see cref="IsSuppressFinalize"/>；默认返回true</returns>
        protected virtual bool Disposing(bool disposing) => true;

        /// <summary>
        /// 在调用<see cref="Dispose(bool)"/>方法使用true参数时执行一次
        /// </summary>
        protected virtual void IsSuppressFinalize() { }

        /// <summary>
        /// 调用此方法判断是否已被释放，若已释放则引发<see cref="ObjectDisposedException"/>异常
        /// </summary>
        /// <exception cref="ObjectDisposedException">已被释放</exception>
        protected void ThrowIsDispose()
        {
            if (p_isDispose) throw new ObjectDisposedException(Cheng.Properties.Resources.Exception_StreamDisposeText);
        }
        
        #region 封装

        private bool p_isDispose = false;

        /// <summary>
        /// 关闭当前流的所有非托管资源和链接句柄
        /// </summary>
        /// <remarks>若要释放非托管资源，请重写<see cref="DisposeUnmanaged"/>或<see cref="Disposing(bool)"/>方法而不是此方法</remarks>
        public override void Close()
        {
            Dispose(true);
        }

        /// <summary>
        /// 释放或关闭所有非托管资源
        /// </summary>
        /// <param name="disposing">此参数用于是否调用<see cref="GC.SuppressFinalize(object)"/>以禁止析构函数，true调用，false不调用；若要在析构函数中释放非托管资源，请使用true参数</param>
        protected sealed override void Dispose(bool disposing)
        {
            if (p_isDispose) return;
            p_isDispose = true;

            DisposeUnmanaged();

            var b = Disposing(disposing);

            base.Dispose(disposing);

            if ((disposing && b))
            {
                GC.SuppressFinalize(this);
                IsSuppressFinalize();
            }
        }

        #endregion

        #endregion

    }

}
