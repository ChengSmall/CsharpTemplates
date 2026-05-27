using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cheng.Threads
{

    /// <summary>
    /// 单线程队列事件委托
    /// </summary>
    /// <param name="thread">引发事件的单线程池实例</param>
    public delegate void SingleThreadAction(SingleThreadTasks thread);

    /// <summary>
    /// 单线程队列事件委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="thread">引发事件的单线程池实例</param>
    /// <param name="args">事件参数</param>
    public delegate void SingleThreadAction<in T>(SingleThreadTasks thread, T args);

    /// <summary>
    /// 循环单线程任务队列引发的异常
    /// </summary>
    public class SingleThreadException : Exception
    {

        public SingleThreadException() : base()
        {
        }

        public SingleThreadException(string message) : base(message)
        {
        }

        public SingleThreadException(string message, Exception exception) : base(message, exception)
        {
        }

    }

}
#if DEBUG
#endif