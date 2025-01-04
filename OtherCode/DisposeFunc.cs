using System;

namespace Cheng
{
    /// <summary>
    /// 释放方法模板
    /// </summary>
    class DisposeFunc : IDisposable
    {

        #region 释放
        /// <summary>
        /// 在派生类重写此方法释放资源
        /// </summary>
        protected virtual void Releaseresources() { }

        #region
        private bool p_isDisposed;
        /// <summary>
        /// 该实例是否已被释放
        /// </summary>
        public bool IsDispose
        {
            get => p_isDisposed;
        }
        /// <summary>
        /// 调用此方法释放资源
        /// </summary>
        /// <param name="suppressFinalize">释放后是否取消析构，true取消，false不取消；若在析构函数中调用则应使用false</param>
        protected void Dispose(bool suppressFinalize)
        {
            if (!p_isDisposed)
            {
                p_isDisposed = true;

                Releaseresources();

                if (suppressFinalize)
                {
                    GC.SuppressFinalize(this);
                }
                
            }
        }
        /// <summary>
        /// 调用此方法释放相关资源
        /// </summary>
        public virtual void Close()
        {
            Dispose(true);
        }
        void IDisposable.Dispose()
        {
        }
        #endregion

        #endregion

    }
}
