using System;
using System.Collections.Concurrent;
using System.Collections;

using System.Threading;
using System.Threading.Tasks;

namespace Cheng.Threads
{

    /// <summary>
    /// 提供同步<![CDATA[async/await]]>单线程代码执行的上下文
    /// </summary>
    /// <remarks>
    /// <para>
    /// 使用<see cref="SynchronizationContext.SetSynchronizationContext(SynchronizationContext)"/>设置上下文对象，在消息循环线程内循环调用<see cref="PumpInvoke"/>，即可让 <![CDATA[async/await]]> 异步代码在在同一个线程内执行；用于从零构建单线程消息循环的模型
    /// </para>
    /// </remarks>
    public sealed class SingleThreadedSynchronizationContext : SynchronizationContext, IDisposable
    {

        #region

        /// <summary>
        /// 实例化一个单线程同步上下文
        /// </summary>
        public SingleThreadedSynchronizationContext()
        {
            p_queue = new ConcurrentQueue<(SendOrPostCallback, object)>();
            p_mres = new ManualResetEventSlim(false);
            p_isDispose = false;
        }

        private ConcurrentQueue<(SendOrPostCallback, object)> p_queue;
        private ManualResetEventSlim p_mres;

        #endregion

        #region 释放

#if DEBUG
        /// <summary>
        /// 释放代码内终止了析构函数时调用该方法
        /// </summary>
        /// <remarks>
        /// <para>当调用清理函数<see cref="Dispose(bool)"/>参数为true，且<see cref="Disposeing(bool)"/>返回值为true时，会调用该方法一次；</para>
        /// </remarks>
#endif
        private void IsSuppressFunalize() { }

#if DEBUG
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
#endif
        private bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                p_mres.Dispose();
            }
            p_mres = null;
            return true;
        }

        /// <summary>
        /// 当前实例是否已被释放
        /// </summary>
        public bool IsDispose => p_isDispose;

        #region 封装

        private bool p_isDispose;

        /// <summary>
        /// 调用该方法清理非托管资源
        /// </summary>
        public void Close()
        {
            Dispose(true);
        }

#if DEBUG
        /// <summary>
        /// 调用此方法清理非托管资源
        /// </summary>
        /// <param name="disposed">是否释放托管资源并停止析构方法；
        /// <para>参数为true时，在资源释放后若<see cref="Disposeing(bool)"/>的返回值为true，则会使用<see cref="GC.SuppressFinalize(object)"/>禁止该对象的对象终结器并调用<see cref="IsSuppressFunalize"/>；<br/>
        /// 若参数是false，则仅释放资源，且不会调用<see cref="IsSuppressFunalize"/>；一般在析构函数中调用时使用false</para>
        /// </param>
        /// <param name="notSuppressFinalize">如果该参数为true，则无论如何都不会使用<see cref="GC.SuppressFinalize(object)"/>来终止析构函数；参数为false则正常运行</param>
#endif
        private void Dispose(bool disposed, bool notSuppressFinalize)
        {
            if (p_isDispose) return;
            p_isDispose = true;

            //----释放----
            bool flag = Disposeing(disposed);
            //----释放----

            if (disposed && flag && (!notSuppressFinalize))
            {
                GC.SuppressFinalize(this);
                IsSuppressFunalize();
            }

        }

#if DEBUG
        /// <summary>
        /// 调用此方法清理非托管资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源并停止析构方法；
        /// <para>参数为true时，在资源释放后若<see cref="Disposeing(bool)"/>的返回值为true，则会使用<see cref="GC.SuppressFinalize(object)"/>禁止该对象的对象终结器并调用<see cref="IsSuppressFunalize"/>；<br/>
        /// 若参数是false，则仅释放资源，且不会调用<see cref="IsSuppressFunalize"/>；一般在析构方法中调用时使用false</para>
        /// </param>
#endif
        private void Dispose(bool disposing)
        {
            Dispose(disposing, false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 调用该函数，以此在实例资源已释放时引发<see cref="ObjectDisposedException"/>异常
        /// </summary>
        private void ThrowObjectDisposeException()
        {
            if (p_isDispose) throw new ObjectDisposedException(nameof(SingleThreadedSynchronizationContext));
        }

        #endregion

        #endregion

        #region 实现

        public override void Post(SendOrPostCallback d, object state)
        {
            p_queue.Enqueue((d, state));
        }

        private sealed class c_SendCB
        {
            internal c_SendCB(SendOrPostCallback d, object state, ManualResetEventSlim done)
            {
                this.d = d;
                this.state = state;
                this.done = done;
            }

            internal SendOrPostCallback d;
            internal object state;
            internal ManualResetEventSlim done;
        }

        internal void fcb_PostCallback(object state)
        {
            var s = (c_SendCB)state;
            try
            {
                s.d?.Invoke(s.state);
            }
            finally
            {
                s.done.Set();
            }
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            if (Current == this)
            {
                // 已在主线程直接执行
                d?.Invoke(state);
                return;
            }

            // 投递后阻塞
            p_mres.Reset();
            Post(fcb_PostCallback, new c_SendCB(d, state, p_mres));
            p_mres.Wait();

            //using (var done = new ManualResetEventSlim(false))
            //{
            //    Post(fcb_PostCallback, new c_SendCB(d, state, done));
            //    done.Wait();
            //}
        }

        #endregion

        /// <summary>
        /// 阻塞调用消息泵更新
        /// </summary>
        /// <remarks>
        /// <para>需要在线程内循环调用的消息泵更新</para>
        /// </remarks>
        /// <exception cref="ObjectDisposedException">已释放</exception>
        public void PumpInvoke()
        {
            ThrowObjectDisposeException();
            while (p_queue.TryDequeue(out var item))
            {
                item.Item1?.Invoke(item.Item2);
            }
        }

    }

}
#if DEBUG
#endif