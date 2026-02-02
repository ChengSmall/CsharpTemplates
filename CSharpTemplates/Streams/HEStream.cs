using Cheng.Memorys;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Cheng.Streams
{

    /// <summary>
    /// 二次封装的流
    /// </summary>
    public abstract class HEStream : Stream, IIsDisposed
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
            if (p_isDispose) throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// 调用此方法判断是否已被释放，若已释放则引发<see cref="ObjectDisposedException"/>异常
        /// </summary>
        /// <param name="objectName">已释放对象的名称</param>
        /// <exception cref="ObjectDisposedException">已被释放</exception>
        protected void ThrowIsDispose(string objectName)
        {
            if (p_isDispose) throw new ObjectDisposedException(objectName:objectName);
        }

        /// <summary>
        /// 调用此方法判断是否已被释放，若已释放则引发<see cref="ObjectDisposedException"/>异常
        /// </summary>
        /// <param name="objectName">已释放对象的名称</param>
        /// <param name="errorMessage">异常消息参数</param>
        /// <exception cref="ObjectDisposedException">已被释放</exception>
        protected void ThrowIsDispose(string objectName, string errorMessage)
        {
            if (p_isDispose) throw new ObjectDisposedException(objectName:objectName, message:errorMessage);
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

        #region 扩展

        /// <summary>
        /// 从流中读取数据到指定地址并推进流对象的位置
        /// </summary>
        /// <param name="buffer">数据接收的内存首地址</param>
        /// <param name="count">要读取的字节数</param>
        /// <returns>此次读取实际的字节数</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">无读取权限</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public unsafe virtual int ReadToAddress(byte* buffer, int count)
        {
            ThrowIsDispose();
            int c = 0;

            while (c < count)
            {
                var re = ReadByte();

                if(re == -1) break;

                buffer[c++] = (byte)re;
            }

            return c;
        }

        /// <summary>
        /// 写入指定地址中的数据
        /// </summary>
        /// <param name="buffer">要写入的数据首地址</param>
        /// <param name="count">要写入的字节数</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="IOException">IO错误</exception>
        /// <exception cref="NotSupportedException">无写入权限</exception>
        /// <exception cref="ObjectDisposedException">流已释放</exception>
        /// <exception cref="Exception">其它错误</exception>
        public unsafe virtual void WriteToAddress(byte* buffer, int count)
        {
            ThrowIsDispose();

            for (int i = 0; i < count; i++)
            {
                this.WriteByte(buffer[i]);
            }
        }

        /// <summary>
        /// 当前流是否属于对<see cref="Stream"/>对象的外部封装操作
        /// </summary>
        public virtual bool CanInternalStream => InternalBaseStream != null;

        /// <summary>
        /// 内部封装的流对象，不存在为null；若对象已释放，也返回null
        /// </summary>
        public virtual Stream InternalBaseStream => null;

        #endregion

    }

}
