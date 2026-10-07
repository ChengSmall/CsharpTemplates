using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

using Cheng.DataStructure.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Cheng.Threads
{

    /// <summary>
    /// 单线程后台任务循环
    /// </summary>
    public sealed class SingleThreadTasks : TaskScheduler, IDisposable
    {

        #region 初始化

        /// <summary>
        /// 实例化循环单线程任务队列
        /// </summary>
        public SingleThreadTasks()
        {
            f_init(true, ThreadPriority.Normal, 0, ApartmentState.MTA, null);
            f_initPar();
        }

        /// <summary>
        /// 实例化循环单线程任务队列
        /// </summary>
        /// <param name="isBackground">指示是否为后台线程</param>
        /// <param name="state">指定线程单元状态</param>
        /// <exception cref="Exception">异常</exception>
        public SingleThreadTasks(bool isBackground, ApartmentState state)
        {
            f_init(isBackground, ThreadPriority.Normal, 0, state, null);
            f_initPar();
        }

        /// <summary>
        /// 实例化循环单线程任务队列
        /// </summary>
        /// <param name="isBackground">指示是否为后台线程</param>
        /// <param name="state">指定线程单元状态</param>
        /// <param name="priority">指定线程调度优先级</param>
        /// <exception cref="Exception">异常</exception>
        public SingleThreadTasks(bool isBackground, ApartmentState state, ThreadPriority priority)
        {
            f_init(isBackground, priority, 0, state, null);
            f_initPar();
        }

        /// <summary>
        /// 实例化循环单线程任务队列
        /// </summary>
        /// <param name="isBackground">指示是否为后台线程</param>
        /// <param name="state">指定线程单元状态</param>
        /// <param name="priority">指定线程调度优先级</param>
        /// <param name="maxStackSize">
        /// <para>线程要使用的最大堆栈大小（以字节为单位）</para>
        /// <para>如果为0，则使用可执行文件的文件头中指定的默认最大堆栈大小</para>
        /// <para>重要事项：对于部分受信任的代码，如果 maxStackSize大于默认堆栈大小，则将其忽略不引发异常</para>
        /// </param>
        /// <exception cref="Exception">异常</exception>
        public SingleThreadTasks(bool isBackground, ApartmentState state, ThreadPriority priority, int maxStackSize)
        {
            f_init(isBackground, priority, maxStackSize, state, null);
            f_initPar();
        }

        /// <summary>
        /// 实例化循环单线程任务队列
        /// </summary>
        /// <param name="isBackground">指示是否为后台线程</param>
        /// <param name="state">指定线程单元状态</param>
        /// <param name="priority">指定线程调度优先级</param>
        /// <param name="maxStackSize">
        /// <para>线程要使用的最大堆栈大小（以字节为单位）</para>
        /// <para>如果为0，则使用可执行文件的文件头中指定的默认最大堆栈大小</para>
        /// <para>重要事项：对于部分受信任的代码，如果 maxStackSize大于默认堆栈大小，则将其忽略不引发异常</para>
        /// </param>
        /// <param name="name">设置线程名称</param>
        /// <exception cref="Exception">异常</exception>
        public SingleThreadTasks(bool isBackground, ApartmentState state, ThreadPriority priority, int maxStackSize, string name)
        {
            f_init(isBackground, priority, maxStackSize, state, name);
            f_initPar();
        }

        private void f_init(bool isBack, ThreadPriority priority, int stackMax, ApartmentState state, string name)
        {
            p_isBack = isBack;
            p_stackMax = stackMax;
            p_state = state;
            p_priority = priority;
            p_threadName = name;
            f_initNewThreadObj();
        }

        private void f_initPar()
        {
            p_tasks = new ImmediatelyQueue<Task>();
            p_buffer = new ImmediatelyQueue<Task>();
            p_start = false;
            p_close = false;
            p_running = false;
            p_vacantTime = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            p_mres = new ManualResetEventSlim(false);
            p_isDispose = false;
            //System.Threading.ManualResetEvent;
            //System.Threading.CountdownEvent;
            //System.Threading.AutoResetEvent;
            //System.Threading.ManualResetEventSlim;
        }

#if DEBUG
        /// <summary>
        /// 初始化新线程对象
        /// </summary>
#endif
        private void f_initNewThreadObj()
        {
            p_thread = new Thread(f_loopThreadFunc, p_stackMax);
            p_thread.SetApartmentState(p_state);
            p_thread.Priority = p_priority;
            p_thread.IsBackground = p_isBack;
            if (!string.IsNullOrEmpty(p_threadName)) p_thread.Name = p_threadName;
        }

        #endregion

        #region 参数

        #region 线程实例
        private int p_stackMax;
        private ApartmentState p_state;
        private ThreadPriority p_priority;
        private string p_threadName;

        private Thread p_thread;
        private ManualResetEventSlim p_mres;
#if DEBUG
        /// <summary>
        /// 任务队列
        /// </summary>
#endif
        private ImmediatelyQueue<Task> p_tasks;

#if DEBUG
        /// <summary>
        /// 任务等待缓存
        /// </summary>
#endif
        private ImmediatelyQueue<Task> p_buffer;

        #endregion

        #region 控制参数

        private TimeSpan p_vacantTime;

        private bool p_isBack;

#if DEBUG

#endif
        private bool p_start;

#if DEBUG
        /// <summary>
        /// 是否存在运行的单线程循环
        /// </summary>
#endif
        private bool p_running;

        private bool p_close;

        #endregion

        #endregion

        #region 功能

        #region 事件

        /// <summary>
        /// 在执行队列中的任务引发异常时的事件
        /// </summary>
        public event SingleThreadAction<Exception> TaskThrowExceptionEvent;

        /// <summary>
        /// 当安全退出单线程循环时引发的事件
        /// </summary>
        public event SingleThreadAction TaskOverEvent;

        #endregion

        #region 封装

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

#if DEBUG
        /// <summary>
        /// 从缓冲区里提取待执行列表
        /// </summary>
        /// <returns>true表示成功从中提取任务到列表</returns>
#endif
        private bool f_addBufferTask()
        {
            bool re = false;
            lock (p_buffer)
            {
                int count = 0;
                int end = Math.Min(p_buffer.Count, 3);
                lock (p_tasks)
                {
                    while (count < end)
                    {
                        if (p_buffer.TryDequeue(out var st))
                        {
                            if (st != null)
                            {
                                p_tasks.Enqueue(st);
                                re = true;
                            }
                        }
                        else
                        {
                            break;
                        }
                        count++;
                    }

                }
            }
            return re;
        }

#if DEBUG
        /// <summary>
        /// 将执行列表的任务依次执行直至为空
        /// </summary>
        /// <returns>false表示此次没有任何任务</returns>
#endif
        private bool f_onceTaskLoop()
        {

            lock (p_tasks)
            {
                int count = p_tasks.Count;

                if (count == 0) return false;

                Task task;

                while (p_tasks.TryDequeue(out task))
                {
                    try
                    {
                        TryExecuteTask(task);
                    }
                    catch (Exception ex)
                    {
                        this.TaskThrowExceptionEvent?.Invoke(this, ex);
                    }
                    Thread.Sleep(0);
                }
            }

            return true;
        }

#if DEBUG
        /// <summary>
        /// 单线程循环核心函数
        /// </summary>
#endif
        private void f_loopThreadFunc()
        {
            p_start = true;
            p_running = true;
            bool b1, b2;
            while ((!p_isDispose))
            {
                // 添加待运行任务
                b1 = f_addBufferTask();
                b2 = false;
                if (b1)
                {
                    b2 = f_onceTaskLoop();
                }

                if (b1 || b2)
                {
                    // 存在任务
                    Thread.Sleep(0);
                }
                else
                {
                    if (!p_running)
                    {
                        break;
                    }
                    // 不存在任务
                    if (p_isDispose)
                    {
                        Thread.Sleep(p_vacantTime);
                    }
                    else
                    {
                        p_mres.Wait(p_vacantTime);
                        p_mres.Reset();
                    }
                }
            }

            TaskOverEvent?.Invoke(this);
            p_running = false;
            p_close = true;
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            if (Debugger.IsAttached)
            {
                List<Task> list = new List<Task>();
                lock (p_buffer)
                {
                    list.AddRange(p_buffer);
                }
                lock (p_tasks)
                {
                    list.AddRange(p_tasks);
                }
                return list;
            }
            throw new NotSupportedException();
        }

        protected override void QueueTask(Task task)
        {
            if (!p_start) throw new TaskSchedulerException(new SingleThreadException());
            if (p_start && (!p_running))
            {
                throw new TaskSchedulerException(new SingleThreadException());
            }
            if (p_close)
            {
                throw new TaskSchedulerException(new SingleThreadException());
            }
            if (task is null) throw new ArgumentNullException();

            lock (p_buffer)
            {
                p_buffer.Enqueue(task);
                p_mres?.Set();
            }
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        protected override bool TryDequeue(Task task)
        {
            if (task is null) throw new ArgumentNullException();
            lock (p_buffer)
            {
                int length = p_buffer.Count;
                for (int i = 0; i < length; i++)
                {
                    var pt = p_buffer.f_getElement(i);
                    if(pt == task)
                    {
                        p_buffer.f_setElement(i, null);
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region 线程参数

        /// <summary>
        /// 线程是否已开启
        /// </summary>
        public bool IsStart
        {
            get => p_start;
        }

        /// <summary>
        /// 线程是否正在运行
        /// </summary>
        public bool Running
        {
            get => p_start && (!p_close);
        }

        /// <summary>
        /// 线程是否已结束
        /// </summary>
        public bool IsEnd
        {
            get => p_close;
        }

        /// <summary>
        /// 线程是否正在关闭
        /// </summary>
        /// <returns>
        /// <para>当参数为true时，表示线程正在执行最后剩余的任务，当执行完所有任务后，引发<see cref="TaskOverEvent"/>事件，随后线程将会关闭；除此之外该参数永远为false</para>
        /// </returns>
        public bool Closing
        {
            get => p_start && (!p_running);
        }

        /// <summary>
        /// 实际运行的线程状态
        /// </summary>
        public System.Threading.ThreadState ThreadState
        {
            get => p_thread.ThreadState;
        }

        /// <summary>
        /// 托管线程标识符
        /// </summary>
        public int ManagedThreadId
        {
            get => p_thread.ManagedThreadId;
        }

        /// <summary>
        /// 线程被设置的名称
        /// </summary>
        public string Name
        {
            get => p_thread.Name;
        }

        /// <summary>
        /// 该线程是否为后台线程
        /// </summary>
        public bool IsBackground
        {
            get => p_isBack;
        }

        /// <summary>
        /// 访问或设置该线程的调度优先级
        /// </summary>
        public ThreadPriority Priority
        {
            get
            {
                return p_thread.Priority;
            }
            set
            {
                if (p_close) throw new ThreadStateException();
                p_thread.Priority = value;
            }
        }

        /// <summary>
        /// 在无任务列表时线程进行一次休眠的最大时间；该值默认500毫秒
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public TimeSpan VacantTime
        {
            get => p_vacantTime;
            set
            {
                if(value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException();
                }
                p_vacantTime = value;
            }
        }

        public override int MaximumConcurrencyLevel => 1;

        #endregion

        #region 运行

        /// <summary>
        /// 开始运行单线程循环任务
        /// </summary>
        /// <exception cref="SingleThreadException">线程已启动或结束</exception>
        public void Start()
        {
            if (p_start)
            {
                throw new SingleThreadException("线程已启动或结束");
            }
            p_start = true;
            p_thread.Start();
        }

        /// <summary>
        /// 安全退出单线程循环任务
        /// </summary>
        /// <remarks>
        /// <para>调用该方法安全结束该单线程循环；</para>
        /// <para>当调用后，线程不会立即结束，而是会将剩余任务执行完毕，再结束任务；<see cref="TaskOverEvent"/>事件会在所有任务执行完毕后引发</para>
        /// </remarks>
        /// <exception cref="SingleThreadException">线程已启动或结束</exception>
        public void End()
        {
            if ((!p_start) || p_close)
            {
                throw new SingleThreadException("线程已启动或结束");
            }
            p_running = false;
        }

        /// <summary>
        /// 停止当前线程并等待任务线程结束
        /// </summary>
        /// <returns>
        /// <para>返回true表示线程已结束；false表示线程并未关闭，无法等待</para>
        /// </returns>
        /// <exception cref="SingleThreadException">线程未开始</exception>
        public bool Wait()
        {
            if (!p_start)
            {
                throw new SingleThreadException("线程未开始");
            }

            if (p_running) return false;

            if (p_close) return true;

            p_thread.Join();

            return true;
        }

        /// <summary>
        /// 向任务队列添加一个新任务等待运行
        /// </summary>
        /// <param name="action">要执行的任务委托</param>
        /// <returns>准备执行的任务</returns>
        /// <exception cref="SingleThreadException">线程循环未运行或已关闭</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="InvalidOperationException">非有效状态</exception>
        /// <exception cref="TaskSchedulerException">无法将此任务排入队列</exception>
        public Task AddTask(Action action)
        {
            Task task = new Task(action);
            task.Start(this);
            return task;
        }

        /// <summary>
        /// 向任务队列添加一个新任务等待运行
        /// </summary>
        /// <param name="action">要执行的任务委托</param>
        /// <param name="state">执行的参数</param>
        /// <returns>准备执行的任务</returns>
        /// <exception cref="SingleThreadException">线程循环未运行或已关闭</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="InvalidOperationException">非有效状态</exception>
        /// <exception cref="TaskSchedulerException">无法将此任务排入队列</exception>
        public Task AddTask(Action<object> action, object state)
        {
            Task task = new Task(action, state);
            task.Start(this);
            return task;
        }

        /// <summary>
        /// 向任务队列添加一个新任务等待运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">要执行的任务委托</param>
        /// <returns>准备执行的任务</returns>
        /// <exception cref="SingleThreadException">线程循环未运行或已关闭</exception>
        /// <exception cref="ArgumentException">参数是null</exception>
        /// <exception cref="InvalidOperationException">非有效状态</exception>
        /// <exception cref="TaskSchedulerException">无法将此任务排入队列</exception>
        public Task<T> AddTask<T>(Func<T> func)
        {
            var task = new Task<T>(func);
            task.Start(this);
            return task;
        }

        /// <summary>
        /// 向任务队列添加一个新任务等待运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">要执行的任务委托</param>
        /// <param name="state">执行的参数</param>
        /// <returns>准备执行的任务</returns>
        /// <exception cref="SingleThreadException">线程循环未运行或已关闭</exception>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="InvalidOperationException">非有效状态</exception>
        /// <exception cref="TaskSchedulerException">无法将此任务排入队列</exception>
        public Task<T> AddTask<T>(Func<object, T> func, object state)
        {
            var task = new Task<T>(func, state);
            task.Start(this);
            return task;
        }

        #endregion

        #region 功能

        /// <summary>
        /// 创建并运行一个单线程后台任务
        /// </summary>
        /// <returns>运行的单线程后台任务</returns>
        public static SingleThreadTasks CreateBackgroundTaskScheduler()
        {
            var t = new SingleThreadTasks(true, ApartmentState.MTA, ThreadPriority.Normal);
            t.Start();
            return t;
        }

        #endregion

        #endregion

    }

}
#if DEBUG
#endif