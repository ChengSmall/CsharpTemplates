using System;

namespace Cheng.ButtonTemplates
{

    /// <summary>
    /// 一个需要释放资源的按钮基类
    /// </summary>
    public abstract class ResourceButton : BaseButton , IDisposable
    {

        #region 释放

        /// <summary>
        /// 在派生类重写此方法释放相关资源
        /// </summary>
        protected virtual void Releaseresources() { }

        /// <summary>
        /// 在派生类重写此方法以更自由的控制释放资源时机
        /// </summary>
        /// <param name="suppressFinalize">调用<see cref="Dispose(bool)"/>方法时传入的参数</param>
        /// <returns>返回true表示执行自动取消析构，false则不会执行取消析构GC方法</returns>
        protected virtual bool Disposing(bool suppressFinalize)
        {
            return true;
        }

        #region

        protected ResourceButton()
        {
            p_isDisposed = false;
        }

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
            if (p_isDisposed) return;

            p_isDisposed = true;

            Releaseresources();

            var b = Disposing(suppressFinalize);
            if (suppressFinalize && b)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 调用此方法释放按钮的相关资源
        /// </summary>
        public virtual void Close()
        {
            Dispose(true);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion

        #endregion

    }

}
