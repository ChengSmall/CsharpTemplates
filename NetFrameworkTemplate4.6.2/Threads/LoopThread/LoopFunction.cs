using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cheng.Memorys;

namespace Cheng.LoopThreads
{

    /// <summary>
    /// 表示一个封装的循环执行函数
    /// </summary>
    public class LoopFunction : SafreleaseUnmanagedResources
    {

        #region 释放

        /// <summary>
        /// 注销事件系统并释放非托管资源
        /// </summary>
        /// <remarks>在派生类重写需要调用基实现</remarks>
        /// <param name="disposeing"></param>
        /// <returns>返回为true</returns>
        protected override bool Disposeing(bool disposeing)
        {
            if (disposeing)
            {
                p_exceptionEvent = null;
                p_fixedUpdateEvent = null;
                p_fixedUpdateScaleEvent = null;
                p_initEvent = null;
                p_updateEvent = null;
                p_yieldEnumatorMoveEvent = null;
            }

            return true;
        }

        /// <summary>
        /// 注销所有事件并释放非托管资源
        /// </summary>
        public override void Close()
        {
            Dispose(true);
        }

        #endregion

        #region 构造

        /// <summary>
        /// 实例化一个循环线程
        /// </summary>
        public LoopFunction()
        {
            f_init();
        }

        private void f_init()
        {
            p_timeScale = 1;

            p_runTime = TimeSpan.Zero;
            p_frame = 0;
            p_nowFrameTime = TimeSpan.Zero;
            p_runScaleTime = TimeSpan.Zero;
            p_nowScaleFrameTime = TimeSpan.Zero;

            p_fixedUpdateTimeSpan = FixedUpdateTimeSpanDefault;

            p_yieldAddBuffer = new Stack<IEnumerator>();
            p_yieldList = new List<YieldEnumator>();

            p_fps = 0;

            //p_waitTimer = new Stopwatch();
            //p_threadEventLock = new object();

            p_threadSafe_initEvent = new object();
            p_threadSafe_updateEvent = new object();
            p_threadSafe_lateUpdateEvent = new object();
            p_threadSafe_fixedUpdateScaleEvent = new object();
            p_threadSafe_fixedUpdateEvent = new object();
            p_threadSafe_exceptionEvent = new object();
            p_threadSafe_yieldEnumatorMoveEvent = new object();
            p_threadSafe_loopExitEvent = new object();
        }

        #endregion

        #region 参数

        #region 时间系统

        /// <summary>
        /// fixed事件的间隔
        /// </summary>
        protected TimeSpan p_fixedUpdateTimeSpan;

        #region 时间判断

        /// <summary>
        /// 每调用fixed事件后的时间
        /// </summary>
        protected DateTime f_fixedLateCallTime;

        /// <summary>
        /// 每调用fixed缩放事件后的时间
        /// </summary>
        protected DateTime f_fixedLateCallScaleTime;

        /// <summary>
        /// 上一帧时间
        /// </summary>
        private DateTime p_lastTime;

        /// <summary>
        /// 当前帧时间
        /// </summary>
        internal protected DateTime p_nowTime;

        /// <summary>
        /// 上一帧的缩放时间
        /// </summary>
        protected DateTime p_lastScaleTime;

        /// <summary>
        /// 当前帧缩放时间
        /// </summary>
        internal protected DateTime p_nowScaleTime;

        /// <summary>
        /// 当前帧时间间隔
        /// </summary>
        protected TimeSpan p_nowFrameTime;

        /// <summary>
        /// 运行帧数
        /// </summary>
        protected ulong p_frame;

        /// <summary>
        /// 运行时总时间
        /// </summary>
        protected TimeSpan p_runTime;

        /// <summary>
        /// 时间缩放
        /// </summary>
        protected double p_timeScale;

        /// <summary>
        /// 具有缩放的时间运行时长
        /// </summary>
        protected TimeSpan p_runScaleTime;

        /// <summary>
        /// 具有缩放的帧时间间隔
        /// </summary>
        protected TimeSpan p_nowScaleFrameTime;

        /// <summary>
        /// 开始的时间点
        /// </summary>
        protected DateTime p_startTime;

        //private Stopwatch p_waitTimer;

        #endregion

        /// <summary>
        /// 动态时间间隔
        /// </summary>
        protected TimeSpan p_dyeltaTime;

        /// <summary>
        /// fps管控
        /// </summary>
        protected int p_fps;

        #endregion

        #region 事件委托

        private object p_threadSafe_updateEvent;
        private LoopThreadAction p_updateEvent;

        private object p_threadSafe_lateUpdateEvent;
        private LoopThreadAction p_lateUpdateEvent;

        private object p_threadSafe_fixedUpdateEvent;
        private LoopThreadAction p_fixedUpdateEvent;

        private object p_threadSafe_fixedUpdateScaleEvent;
        private LoopThreadAction p_fixedUpdateScaleEvent;

        private object p_threadSafe_initEvent;
        private LoopThreadAction p_initEvent;

        private object p_threadSafe_exceptionEvent;
        private LoopThreadAction<Exception> p_exceptionEvent;

        private object p_threadSafe_yieldEnumatorMoveEvent;
        private LoopThreadAction<object> p_yieldEnumatorMoveEvent;

        private object p_threadSafe_loopExitEvent;
        private LoopThreadAction p_loopExitEvent;
        #endregion

        #region 协程

        /// <summary>
        /// 协程添加缓冲区
        /// </summary>
        private Stack<IEnumerator> p_yieldAddBuffer;

        /// <summary>
        /// 协程运作集合
        /// </summary>
        private List<YieldEnumator> p_yieldList;

        #endregion

        #region 条件参数
        private bool f_start;

        private bool f_loop;
        #endregion

        #endregion

        #region 参数访问

        #region 线程调用委托

        /// <summary>
        /// 获取要执行循环的方法委托
        /// </summary>
        public ThreadStart LoopFunc
        {
            get => LoopFunctionMethod;
        }

        /// <summary>
        /// 获取要执行循环的方法委托
        /// </summary>
        public Action LoopAction
        {
            get => LoopFunctionMethod;
        }

        #endregion

        #region 时间参数

        /// <summary>
        /// Fixed事件的默认执行时间间隔
        /// </summary>
        public static TimeSpan FixedUpdateTimeSpanDefault
        {
            get => TimeSpan.FromSeconds(0.05);
        }

        /// <summary>
        /// 此循环开启后的总运行时间
        /// </summary>
        public TimeSpan RunTime
        {
            get
            {
                return p_runTime;
            }
        }

        /// <summary>
        /// 此循环开启后的总运行缩放时间
        /// </summary>
        public TimeSpan ScaleRunTime
        {
            get
            {
                return p_runScaleTime;
            }
        }

        /// <summary>
        /// 当前帧的时间间隔
        /// </summary>
        public TimeSpan FrameTime
        {
            get => p_nowFrameTime;
        }

        /// <summary>
        /// 当前帧的缩放时间间隔
        /// </summary>
        public TimeSpan ScaleFrameTime
        {
            get => p_nowScaleFrameTime;
        }

        /// <summary>
        /// 获取或设置时间缩放倍率，默认为1
        /// </summary>
        /// <remarks>
        /// 缩放倍率控制着缩放时间流动速度，缩放时间运行速度 = 实际运行速度 * 缩放时间倍率；<br/>
        /// 值越大，缩放时间流速越快，反之则越慢；当值为0时，缩放时间将不再流动
        /// <para>
        /// 会被时间缩放倍率影响的参数有：<see cref="Scale"/>、<see cref="ScaleFrameTime"/>、<see cref="ScaleRunTime"/>、<see cref="FixedUpdateScaleEvent"/>
        /// </para>
        /// <para>注意：缩放时间过快会导致<see cref="TimeSpan"/>和<see cref="DateTime"/>实例的值超出范围，建议控制在1000以内</para>
        /// </remarks>
        /// <value>值不得小于0</value>
        /// <exception cref="ArgumentOutOfRangeException">值小于0</exception>
        public double Scale
        {
            get => p_timeScale;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value", "给定的值小于0");
                p_timeScale = value;
            }
        }

        /// <summary>
        /// 获取此独立循环已运行了多少帧；一次循环表示一帧，若还未运行则为0
        /// </summary>
        public ulong Frame
        {
            get => p_frame;
        }

        /// <summary>
        /// 访问或设置<see cref="FixedUpdateEvent"/>和<see cref="FixedUpdateScaleEvent"/>事件的时间间隔
        /// </summary>
        /// <value>参数不得小于或等于0，默认值为<see cref="FixedUpdateTimeSpanDefault"/></value>
        /// <exception cref="ArgumentOutOfRangeException">值小于0</exception>
        public TimeSpan FixedTime
        {
            get => p_fixedUpdateTimeSpan;
            set
            {
                if (value <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("value", "给定的值小于0");
                p_fixedUpdateTimeSpan = value;
            }
        }

        /// <summary>
        /// 访问或设置<see cref="FixedUpdateEvent"/>和<see cref="FixedUpdateScaleEvent"/>事件的时间间隔；单位秒
        /// </summary>
        /// <value>参数不得小于或等于0</value>
        /// <exception cref="ArgumentOutOfRangeException">值小于0</exception>
        public double FixedSeconds
        {
            get => p_fixedUpdateTimeSpan.TotalSeconds;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("value", "给定的值小于0");
                p_fixedUpdateTimeSpan = TimeSpan.FromSeconds(value);
            }
        }

        /// <summary>
        /// 返回从开始执行循环到调用此属性之间，真实的时间间隔
        /// </summary>
        public TimeSpan RealRunTime
        {
            get
            {
                return DateTime.UtcNow - p_startTime;
            }
        }

        /// <summary>
        /// 动态获取此时事件帧的时间间隔
        /// </summary>
        /// <returns>
        /// 此属性用于在循环事件或协程函数内，获取相应的帧间隔时间；<br/>
        /// 当在<see cref="UpdateEvent"/>，<see cref="FixedUpdateEvent"/>事件和用<see cref="AddCoroutine(IEnumerator)"/>添加的协程枚举当中获取此属性时，和<see cref="FrameTime"/>属性相同；当在<see cref="FixedUpdateScaleEvent"/>事件中获取此属性时，和<see cref="ScaleFrameTime"/>属性相同；
        /// <para>若在循环事件外获取此属性，将会随机获取<see cref="FrameTime"/>或<see cref="ScaleFrameTime"/>的值，并不保证连贯性</para>
        /// </returns>
        public TimeSpan DyeltaTime
        {
            get
            {
                return p_dyeltaTime;
            }
        }

        /// <summary>
        /// 限制每秒帧数
        /// </summary>
        /// <value>限制循环每秒帧数，0表示无限制；该值只是一个期望值，实际帧数会有浮动；值默认为0</value>
        /// <exception cref="ArgumentOutOfRangeException">参数小于0</exception>
        public int FramesPerSecond
        {
            get => p_fps;
            set
            {
                //frames per second  FramesPerSecond
                if (value < 0) throw new ArgumentOutOfRangeException();
                p_fps = value;
            }
        }
        #endregion

        #region 事件

        /// <summary>
        /// 在循环开始前发生一次的事件，用于初始化数据
        /// </summary>
        public event LoopThreadAction InitEvent
        {
            add
            {
                lock (p_threadSafe_initEvent)
                {
                    p_initEvent += value;
                }
            }
            remove
            {
                lock (p_threadSafe_initEvent)
                {
                    p_initEvent -= value;
                }
            }
        }

        /// <summary>
        /// 每次循环都会发生一次的事件
        /// </summary>
        public event LoopThreadAction UpdateEvent
        {
            add
            {
                lock (p_threadSafe_updateEvent)
                {
                    p_updateEvent += value;
                }
            }
            remove
            {
                lock (p_threadSafe_updateEvent)
                {
                    p_updateEvent -= value;
                }
            }
        }

        /// <summary>
        /// 所有<see cref="UpdateEvent"/>事件执行后执行该事件
        /// </summary>
        public event LoopThreadAction LateUpdateEvent
        {
            add
            {
                lock (p_threadSafe_lateUpdateEvent)
                {
                    p_lateUpdateEvent += value;
                }
            }
            remove
            {
                lock (p_threadSafe_lateUpdateEvent)
                {
                    p_lateUpdateEvent -= value;
                }
            }
        }

        /// <summary>
        /// 每经过一定时间发生的事件，时间间隔使用<see cref="FixedTime"/>
        /// </summary>
        public event LoopThreadAction FixedUpdateEvent
        {
            add
            {
                lock (p_threadSafe_fixedUpdateEvent)
                {
                    p_fixedUpdateEvent += value;
                }
            }
            remove
            {
                lock (p_threadSafe_fixedUpdateEvent)
                {
                    p_fixedUpdateEvent -= value;
                }
            }
        }

        /// <summary>
        /// 每经过一定的缩放时间发生的事件，时间间隔使用<see cref="FixedTime"/>
        /// </summary>
        public event LoopThreadAction FixedUpdateScaleEvent
        {
            add
            {
                lock (p_threadSafe_fixedUpdateScaleEvent)
                {
                    p_fixedUpdateScaleEvent += value;
                }
            }
            remove
            {
                lock (p_threadSafe_fixedUpdateScaleEvent)
                {
                    p_fixedUpdateScaleEvent -= value;
                }
            }
        }

        /// <summary>
        /// 在循环中引发了异常时发生的异常处理事件
        /// </summary>
        /// <remarks>当此事件没有被注册函数时，在循环中引发的任何异常将不会在内部做任何处理</remarks>
        public event LoopThreadAction<Exception> UnhandledExceptionEvent
        {
            add
            {
                lock (p_threadSafe_exceptionEvent)
                {
                    p_exceptionEvent += value;
                }
            }
            remove
            {
                lock (p_threadSafe_exceptionEvent)
                {
                    p_exceptionEvent -= value;
                }
            }
        }

        /// <summary>
        /// 每次进行一个协程枚举器推进成功后发生，每次发生后会传递此次推进后的值到参数
        /// </summary>
        public event LoopThreadAction<object> YieldEnumatorMoveEvent
        {
            add
            {
                lock (p_threadSafe_yieldEnumatorMoveEvent)
                {
                    p_yieldEnumatorMoveEvent += value;
                }
            }
            remove
            {
                lock (p_threadSafe_yieldEnumatorMoveEvent)
                {
                    p_yieldEnumatorMoveEvent -= value;
                }
            }
        }

        /// <summary>
        /// 在循环退出时引发一次的事件
        /// </summary>
        public event LoopThreadAction LoopExitEvent
        {
            add
            {
                lock (p_threadSafe_loopExitEvent)
                {
                    p_loopExitEvent += value;
                }
            }
            remove
            {
                lock (p_threadSafe_loopExitEvent)
                {
                    p_loopExitEvent -= value;
                }
            }
        }

        #endregion

        #region 循环套件
        
        #endregion

        #endregion

        #region 功能

        #region 循环功能封装

        private void f_yieldListBufferWrite()
        {
            //倒斗协程枚举器
            lock (p_yieldAddBuffer)
            {
                while (p_yieldAddBuffer.Count != 0)
                {
                    p_yieldList.Add(new YieldEnumator(p_yieldAddBuffer.Pop()));
                }
            }
           
        }

        private void f_enumator()
        {
            p_dyeltaTime = p_nowFrameTime;
            f_yieldListBufferWrite();

            int i;
            YieldEnumator ye;
            var list = p_yieldList;
            bool flag;

            //没有则退出循环
            if (list.Count == 0) return;
            object obj;
            lock (list)
            {
                for (i = 0; i < list.Count; )
                {
                    //获取实例
                    ye = list[i];

                        flag = ye.IsNextMove(this);

                        if (!flag)
                        {
                            //不需要
                            i++;
                            continue;
                        }

                        //开始推进
                        flag = ye.MoveNext();

                        if (!flag)
                        {
                            //推进完毕
                            //删除
                            list.RemoveAt(i);
                            continue;
                        }

                        //获取元素
                        obj = ye.Current;
                        YieldEnumatorMoveNext(obj);
                        p_yieldEnumatorMoveEvent?.Invoke(this, obj);

                        if (obj is YieldWaitTime wait)
                        {
                            ye.SetNextTime(p_nowTime + wait.time, false);
                        }
                        else if (obj is YieldWaitScaleTime waits)
                        {
                            ye.SetNextTime(p_nowTime + waits.time, true);
                        }
                        else if (obj is YieldNestEnumator nest)
                        {
                            ye.enumator = new NestEnumator(nest.enumator, ye.enumator);
                        }
                        else
                        {
                            //其它返回值，按帧
                            ye.SetNextTime();
                        }
                        i++;
                    
                        
                }
                
            }
            
        }

        /// <summary>
        /// 每次进行一个协程枚举器推进成功后调用此函数
        /// </summary>
        /// <param name="current">此次推进后的值</param>
        protected virtual void YieldEnumatorMoveNext(object current) { }

        private void f_exception(Exception ex)
        {
            if(p_exceptionEvent == null)
            {
                throw ex;
            }
            p_exceptionEvent.Invoke(this, ex);
        }

        /// <summary>
        /// 第一次循环前调用
        /// </summary>
        protected virtual void LoopStartInvoke() { }

        private void f_loopStartInit()
        {
            p_startTime = DateTime.UtcNow;
            p_lastTime = p_startTime;
            p_nowTime = p_startTime;
            p_lastScaleTime = p_startTime;
            p_nowScaleTime = p_startTime;

            f_fixedLateCallScaleTime = p_startTime;
            f_fixedLateCallTime = p_startTime;

            LoopStartInvoke();
            try
            {
                p_initEvent?.Invoke(this);
            }
            catch (Exception ex)
            {
                f_exception(ex);
            }
            
        }

        private void f_loopFirst()
        {
            DateTime now = DateTime.UtcNow;
            
            p_lastTime = p_nowTime;
            p_nowTime = now;

            p_nowFrameTime = p_nowTime - p_lastTime;

            p_runTime += p_nowFrameTime;

            p_nowScaleFrameTime = new TimeSpan((long)(p_nowFrameTime.Ticks * p_timeScale));


            p_lastScaleTime = p_nowScaleTime;

            p_nowScaleTime += p_nowScaleFrameTime;

            p_runScaleTime += p_nowScaleFrameTime;

            p_dyeltaTime = p_nowFrameTime;

            LoopFirst();
        }

        /// <summary>
        /// 每次循环的调用一次，此函数总是在所有循环事件发生前调用
        /// </summary>
        protected virtual void LoopFirst() { }

        /// <summary>
        /// 每一轮循环调用，以进行帧数限制
        /// </summary>
        protected virtual void EndLoopWaitFPS()
        {
            if (p_fps > 0)
            {
                try
                {
                    //计算等待时间
                    var waitTime = TimeSpan.FromSeconds((1d / p_fps)) - this.p_nowFrameTime;

                    //var sw = new System.Threading.SpinWait();

                    if (waitTime >= TimeSpan.Zero)
                    {
                        ThreadSleep(waitTime);
                    }
                }
                catch (Exception)
                {
                }

            }

        }

        /// <summary>
        /// 调用此函数后，当前线程会进入线程挂起等待，指定等待时间
        /// </summary>
        /// <param name="waitTime">要进行线程等待的时间</param>
        protected virtual void ThreadSleep(TimeSpan waitTime)
        {
            Thread.Sleep(waitTime);
        }

        private void f_loopEnd()
        {
            LoopEnd();

            EndLoopWaitFPS();

            p_frame++;
        }

        /// <summary>
        /// 每次循环的调用一次，此函数总是在所有循环事件发生后调用
        /// </summary>
        protected virtual void LoopEnd() { }

        /// <summary>
        /// 每次<see cref="UpdateEvent"/>事件发生前调用
        /// </summary>
        protected virtual void Update() { }

        private void f_update()
        {
            Update();
            try
            {
                p_updateEvent?.Invoke(this);
            }
            catch (Exception ex)
            {
                f_exception(ex);
            }
        }

        /// <summary>
        /// 所有<see cref="UpdateEvent"/>事件发生后调用
        /// </summary>
        protected virtual void LateUpdate() { }

        private void f_lateUpdate()
        {
            LateUpdate();
            try
            {
                p_lateUpdateEvent?.Invoke(this);
            }
            catch (Exception ex)
            {
                f_exception(ex);
            }
        }

        /// <summary>
        /// 每次<see cref="FixedUpdateEvent"/>事件发生前调用
        /// </summary>
        protected virtual void FixedUpdate() { }

        private void f_fixedUpdate()
        {
            if (p_nowTime - f_fixedLateCallTime >= p_fixedUpdateTimeSpan)
            {
                f_fixedLateCallTime = p_nowTime;

                FixedUpdate();
                try
                {
                    p_fixedUpdateEvent?.Invoke(this);
                }
                catch (Exception ex)
                {
                    f_exception(ex);
                }
            }
        }

        /// <summary>
        /// 每次<see cref="FixedUpdateScaleEvent"/>事件发生前调用
        /// </summary>
        protected virtual void FixedScaleUpdate() { }

        private void f_fixedScaleUpdate()
        {
            if (p_nowScaleTime - f_fixedLateCallScaleTime >= p_fixedUpdateTimeSpan)
            {
                f_fixedLateCallScaleTime = p_nowScaleTime;

                FixedScaleUpdate();
                try
                {
                    p_fixedUpdateScaleEvent?.Invoke(this);
                }
                catch (Exception ex)
                {
                    f_exception(ex);
                }

            }
            
        }

        private void f_updateFunc()
        {
            p_dyeltaTime = p_nowFrameTime;
            f_update();
            f_lateUpdate();
            p_dyeltaTime = p_fixedUpdateTimeSpan;
            f_fixedUpdate();
            p_dyeltaTime = p_nowScaleFrameTime;
            f_fixedScaleUpdate();
            p_dyeltaTime = p_nowFrameTime;
        }

        private void f_exitLoopInvoke()
        {
            p_yieldAddBuffer.Clear();
            p_yieldList.Clear();
            ExitLoop();
            try
            {
                p_loopExitEvent?.Invoke(this);
            }
            catch (Exception ex)
            {
                f_exception(ex);
            }
            
        }

        /// <summary>
        /// 在循环退出时调用一次
        /// </summary>
        protected virtual void ExitLoop() { }

        /// <summary>
        /// 要执行的循环函数
        /// </summary>
        /// <exception cref="LoopStartException">循环已经被执行或已经关闭</exception>
        protected void LoopFunctionMethod()
        {
            if (f_loop)
            {
                throw new LoopStartException("循环已被启动或执行完毕");
            }
            //开始
            f_loop = true;
            f_start = true;

            f_loopStartInit();

            while (f_start)
            {
                //头
                f_loopFirst();

                //帧
                f_updateFunc();

                //枚举器
                f_enumator();

                //尾
                f_loopEnd();
            }

            //退出
            f_exitLoopInvoke();
        }

        #endregion

        #region API

        /// <summary>
        /// 安全退出循环
        /// </summary>
        /// <returns>是否成功退出；成功退出循环返回true，否则返回false</returns>
        public bool Exit()
        {
            if(f_start)
            {
                f_start = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 添加一个协程枚举器
        /// </summary>
        /// <remarks>
        /// 调用此函数添加一个枚举器，循环会在合适的时机开始使用<see cref="IEnumerator.MoveNext"/>方法推进枚举器，直至推进结束；
        /// <para>
        /// 使用yield关键字实现迭代函数，在每一次yield返回值中，有几个特殊实例可控制下一次执行的时间：<br/>
        /// <see cref="YieldWaitTime"/>类实例，指定下次推进的等待时间；<br/>
        /// <see cref="YieldWaitScaleTime"/>类实例，指定下次推进的缩放等待时间；<br/>
        /// <see cref="YieldNestEnumator"/>类实例，指定先推进此实例内的参数，再执行后面的实例；<br/>
        /// 其它返回值表示下一帧推进；
        /// </para>
        /// </remarks>
        /// <param name="enumator">表示协程的枚举器</param>
        /// <exception cref="ArgumentNullException">枚举器为null</exception>
        public void AddCoroutine(IEnumerator enumator)
        {
            if (enumator is null) throw new ArgumentNullException(nameof(enumator));
            lock (p_yieldAddBuffer)
            {
                p_yieldAddBuffer.Push(enumator);
            }
        }

        /// <summary>
        /// 清除所有正在运行的协程枚举器
        /// </summary>
        public void ClearCoroutine()
        {
            lock (p_yieldAddBuffer)
            {
                lock (p_yieldList)
                {
                    p_yieldAddBuffer.Clear();
                    p_yieldList.Clear();
                }
            }         
        }

        /// <summary>
        /// 创建一个此循环线程的计时器
        /// </summary>
        /// <returns>一个计时器实例</returns>
        public LoopThreadTimer CreateTimer()
        {
            return new LoopThreadTimer(this);
        }

        /// <summary>
        /// 创建一个此循环线程的可缩放时间计时器
        /// </summary>
        /// <returns>一个计时器实例</returns>
        public LoopThreadScaleTimer CreateScaleTimer()
        {
            return new LoopThreadScaleTimer(this);
        }

        #endregion

        #endregion

    }

}
