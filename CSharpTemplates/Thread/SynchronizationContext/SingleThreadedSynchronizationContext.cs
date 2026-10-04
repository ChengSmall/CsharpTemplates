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
    public sealed class SingleThreadedSynchronizationContext : SynchronizationContext
    {

        /// <summary>
        /// 实例化一个单线程同步上下文
        /// </summary>
        public SingleThreadedSynchronizationContext()
        {
            p_queue = new ConcurrentQueue<(SendOrPostCallback, object)>();
        }

        private ConcurrentQueue<(SendOrPostCallback, object)> p_queue;

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
            using (var done = new ManualResetEventSlim(false))
            {
                Post(fcb_PostCallback, new c_SendCB(d, state, done));
                done.Wait();
            }
        }

        /// <summary>
        /// 阻塞调用消息泵更新
        /// </summary>
        /// <remarks>
        /// <para>需要在线程内循环调用的消息泵更新</para>
        /// </remarks>
        public void PumpInvoke()
        {
            while (p_queue.TryDequeue(out var item))
            {
                item.Item1?.Invoke(item.Item2);
            }
        }

    }

}
