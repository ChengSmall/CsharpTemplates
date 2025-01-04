using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

namespace Cheng.Threads
{

    /// <summary>
    /// 单线程队列事件委托
    /// </summary>
    /// <param name="pool">引发事件的单线程池实例</param>
    public delegate void SingleThreadAction(SingleThreadTasks pool);

    /// <summary>
    /// 单线程队列事件委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pool">引发事件的单线程池实例</param>
    /// <param name="args">事件参数</param>
    public delegate void SingleThreadAction<in T>(SingleThreadTasks pool, T args);

    /// <summary>
    /// 循环单线程任务队列引发的异常
    /// </summary>
    public class SingleThreadException : Exception
    {

        public SingleThreadException() : base("单线程任务队列异常")
        {
        }

        public SingleThreadException(string message) : base(message)
        {
        }

        public SingleThreadException(string message, Exception exception) : base(message, exception)
        {
        }

    }

    /// <summary>
    /// 循环单线程任务队列
    /// </summary>
    public sealed class SingleThreadTasks
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
        /// <param name="priority">指定线程调度优先级</param>
        /// <param name="state">指定线程单元状态</param>
        /// <param name="maxStackSize">
        /// <para>线程要使用的最大堆栈大小（以字节为单位）</para>
        /// <para>如果为0，则使用可执行文件的文件头中指定的默认最大堆栈大小</para>
        /// <para>重要事项：对于部分受信任的代码，如果 maxStackSize大于默认堆栈大小，则将其忽略不引发异常</para>
        /// </param>
        /// <exception cref="Exception">异常</exception>
        public SingleThreadTasks(bool isBackground, ThreadPriority priority, ApartmentState state, int maxStackSize)
        {
            f_init(isBackground, priority, maxStackSize, state, null);
            f_initPar();
        }

        /// <summary>
        /// 实例化循环单线程任务队列
        /// </summary>
        /// <param name="isBackground">指示是否为后台线程</param>
        /// <param name="priority">指定线程调度优先级</param>
        /// <param name="state">指定线程单元状态</param>
        /// <param name="maxStackSize">
        /// <para>线程要使用的最大堆栈大小（以字节为单位）</para>
        /// <para>如果为0，则使用可执行文件的文件头中指定的默认最大堆栈大小</para>
        /// <para>重要事项：对于部分受信任的代码，如果 maxStackSize大于默认堆栈大小，则将其忽略不引发异常</para>
        /// </param>
        /// <param name="name">设置线程名称</param>
        /// <exception cref="Exception">异常</exception>
        public SingleThreadTasks(bool isBackground, ThreadPriority priority, ApartmentState state, int maxStackSize, string name)
        {
            f_init(isBackground, priority, maxStackSize, state, name);
            f_initPar();
        }

        private void f_init(bool isBack, ThreadPriority priority, int stackMax, ApartmentState state, string name)
        {
            p_thread = new Thread(f_loopThreadFunc, stackMax);
            p_thread.SetApartmentState(state);
            p_thread.Priority = priority;
            p_thread.IsBackground = isBack;
            if((object)name != null) p_thread.Name = name;
            p_isBack = isBack;
        }

        private void f_initPar()
        {
            p_tasks = new List<SingleTask>();
            p_buffer = new List<SingleTask>();
            p_start = false;
            p_close = false;
            p_running = false;
            p_vacantTime = new TimeSpan(20 * TimeSpan.TicksPerMillisecond);
        }

        #endregion

        #region 参数

        #region 线程实例

        private Thread p_thread;

        /// <summary>
        /// 任务队列
        /// </summary>
        private List<SingleTask> p_tasks;

        /// <summary>
        /// 任务等待缓存
        /// </summary>
        private List<SingleTask> p_buffer;

        #endregion

        #region 控制参数

        private TimeSpan p_vacantTime;

        private bool p_isBack;

        private bool p_start;

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

        private void f_addBufferTask()
        {
            //
            lock (p_buffer)
            {
                int count = p_buffer.Count;
                if (count == 0) return;

                for (int i = 0; i < count; i++)
                {
                    p_tasks.Add(p_buffer[i]);
                }

                p_buffer.Clear();
            }

        }

        private bool f_onceTaskLoop()
        {
            int count = p_tasks.Count;

            if (count == 0) return false;

            int i;

            SingleTask task;

            for (i = 0; i < count; i++)
            {

                task = p_tasks[i];
                task.p_start = true;
                
                try
                {

                    task.f_invokeStartEvent();
                    task.f_invokeTask();
                    task.f_invokeOverTaskEvent();
                    
                }
                catch (Exception ex)
                {
                    task.p_abnormalOver = true;
                    this.TaskThrowExceptionEvent?.Invoke(this, ex);
                }

                task.p_over = true;

                p_tasks[i] = null;

                task.p_onList = false;
                
            }

            p_tasks.Clear();
            return true;
        }

        private void f_loopThreadFunc()
        {

            p_start = true;
            bool b = true;
            
            while (p_running || b)
            {
                f_addBufferTask();

                bool b2 = f_onceTaskLoop();
                if (!b2)
                {
                    //无任务
                    lock (p_buffer)
                    {
                        b = p_buffer.Count != 0;
                    }
                    if (p_running)
                    {
                        Thread.Sleep(p_vacantTime);
                    }
                    else
                    {
                        Thread.Sleep(0);
                    }
                }

            }

            TaskOverEvent?.Invoke(this);
            p_close = true;
            
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
        public ThreadState ThreadState
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
        /// 在无任务列表时线程进行一次休眠的时间；该值默认20毫秒
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public TimeSpan VacantTime
        {
            get => p_vacantTime;
            set
            {
                TimeSpan zero = new TimeSpan(0);
                if(value < zero)
                {
                    throw new ArgumentOutOfRangeException();
                }
                p_vacantTime = value;
            }
        }

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
            p_thread.Start();
            p_running = true;
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
        /// 添加一个待执行任务到任务队列
        /// </summary>
        /// <param name="task">要添加的任务</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        /// <exception cref="SingleThreadException">线程已结束</exception>
        public void AddTask(SingleTask task)
        {
            if (p_start && (!p_running))
            {
                throw new SingleThreadException("线程正处于关闭状态");
            }
            if (p_close)
            {
                throw new SingleThreadException("线程已结束");
            }
            if (task is null) throw new ArgumentNullException();
            
            lock (p_buffer)
            {
                p_buffer.Add(task);
                task.p_onList = true;
            }

        }

        /// <summary>
        /// 停止当前线程并等待该线程结束
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

        #endregion

        #region 派生

        #endregion

        #endregion

    }

}
