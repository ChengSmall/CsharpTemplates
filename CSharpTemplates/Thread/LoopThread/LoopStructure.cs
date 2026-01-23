using Cheng.Timers;
using System;
using System.Collections;

namespace Cheng.LoopThreads
{

    #region 事件参数

    /// <summary>
    /// 循环线程的事件委托
    /// </summary>
    /// <typeparam name="T">事件参数类型</typeparam>
    /// <param name="loop">引发此事件的实例</param>
    /// <param name="obj">事件对应的参数</param>
    public delegate void LoopThreadAction<in T>(LoopFunction loop, T obj);

    /// <summary>
    /// 循环线程的事件委托
    /// </summary>
    /// <param name="loop">引发此事件的实例</param>
    public delegate void LoopThreadAction(LoopFunction loop);

    /// <summary>
    /// 循环线程的事件委托
    /// </summary>
    /// <typeparam name="T">事件参数类型</typeparam>
    /// <typeparam name="TL">引发此事件实例的运行时类型</typeparam>
    /// <param name="loop">引发此事件的实例</param>
    /// <param name="args">事件对应的参数</param>
    public delegate void LoopThreadAction<in T, in TL>(TL loop, T args) where TL : LoopFunction;

    #endregion

    #region 协程参数

    /// <summary>
    /// 协程封装
    /// </summary>
    internal class YieldEnumator
    {
        internal YieldEnumator() { }

        public YieldEnumator(IEnumerator enr)
        {
            enumator = enr;
            isNextFrame = true;
        }

        /// <summary>
        /// 下次推进的时间
        /// </summary>
        internal DateTime nextTime;

        internal IEnumerator enumator;

        /// <summary>
        /// 推进时间是否为缩放时间
        /// </summary>
        internal bool nextTimeIsScale;

        /// <summary>
        /// 是为每帧推进
        /// </summary>
        internal bool isNextFrame;

        /// <summary>
        /// 设置下次推进时间
        /// </summary>
        /// <param name="nextTime">下次推进时间</param>
        /// <param name="isScale">是否为缩放时间</param>
        public void SetNextTime(DateTime nextTime, bool isScale)
        {
            isNextFrame = false;
            this.nextTime = nextTime;
            nextTimeIsScale = isScale;
        }

        /// <summary>
        /// 设置下次推进时间为下一帧
        /// </summary>
        public void SetNextTime()
        {
            isNextFrame = true;
        }

        /// <summary>
        /// 判断此帧是否需要推进
        /// </summary>
        /// <param name="loop"></param>
        /// <returns></returns>
        public bool IsNextMove(LoopFunction loop)
        {
            if (isNextFrame) return true;
            if (nextTimeIsScale)
            {
                //缩放
                return nextTime <= loop.p_nowScaleTime;
            }
            //不缩放

            return nextTime <= loop.p_nowTime;
        }

        /// <summary>
        /// 推进枚举器一次
        /// </summary>
        /// <returns>推进函数的返回值</returns>
        public bool MoveNext()
        {
            return enumator.MoveNext();
        }

        /// <summary>
        /// 获取此时迭代的元素
        /// </summary>
        public object Current
        {
            get
            {
                return enumator.Current;
            }
        }

    }

    /// <summary>
    /// 嵌套枚举器
    /// </summary>
    internal class NestEnumator : IEnumerator
    {
        /// <summary>
        /// 枚举器嵌套推进
        /// </summary>
        /// <param name="enr">先推进</param>
        /// <param name="next">先推进完后推进</param>
        public NestEnumator(IEnumerator enr, IEnumerator next)
        {
            enumator = enr;
            nextenr = next;
        }

        internal IEnumerator enumator;
        internal IEnumerator nextenr;

        public object Current
        {
            get
            {
                return enumator.Current;
            }
        }

        public bool MoveNext()
        {
            bool flag;

            flag = enumator.MoveNext();
            if (flag)
            {
                return flag;
            }

            if (enumator is IDisposable)
            {
                ((IDisposable)enumator).Dispose();
            }

            if (nextenr is null)
            {
                return false;
            }

            enumator = nextenr;
            nextenr = null;
            flag = enumator.MoveNext();
            if (!flag)
            {
                if (enumator is IDisposable)
                {
                    ((IDisposable)enumator).Dispose();
                }
                return false;
            }
            return true;
        }

        public void Reset()
        {
        }

    }

    /// <summary>
    /// 协程等待参数，等待指定时间
    /// </summary>
    public class YieldWaitTime
    {

        /// <summary>
        /// 实例化一个协程等待参数，指定下次执行的时间间隔
        /// </summary>
        /// <param name="time">下次执行的时间间隔</param>
        public YieldWaitTime(TimeSpan time)
        {
            if (time < TimeSpan.Zero) time = TimeSpan.Zero;
            this.time = time;
        }

        /// <summary>
        /// 实例化一个协程等待参数，指定下次执行的时间间隔
        /// </summary>
        /// <param name="seconds">下次执行的时间间隔，单位秒</param>
        public YieldWaitTime(double seconds)
        {
            if (seconds < 0) seconds = 0;
            time = TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// 下次时间间隔
        /// </summary>
        internal readonly TimeSpan time;
    }

    /// <summary>
    /// 协程等待参数，等待指定缩放时间
    /// </summary>
    public class YieldWaitScaleTime
    {

        /// <summary>
        /// 实例化一个协程等待参数，指定下次执行的缩放时间间隔
        /// </summary>
        /// <param name="time">下次执行的缩放时间间隔</param>
        public YieldWaitScaleTime(TimeSpan time)
        {
            if (time < TimeSpan.Zero) time = TimeSpan.Zero;
            this.time = time;
        }

        /// <summary>
        /// 实例化一个协程等待参数，指定下次执行的缩放时间间隔
        /// </summary>
        /// <param name="seconds">下次执行的缩放时间间隔，单位秒</param>
        public YieldWaitScaleTime(double seconds)
        {
            if (seconds < 0) seconds = 0;
            time = TimeSpan.FromSeconds(seconds);
        }

        internal readonly TimeSpan time;
    }

    /// <summary>
    /// 协程等待参数，嵌套执行枚举器
    /// </summary>
    public class YieldNestEnumator
    {

        /// <summary>
        /// 实例化一个协程等待参数，执行指定枚举器
        /// </summary>
        /// <param name="enumator">枚举器参数</param>
        /// <exception cref="ArgumentNullException"></exception>
        public YieldNestEnumator(IEnumerator enumator)
        {
            if (enumator is null) throw new ArgumentNullException(nameof(enumator));
            this.enumator = enumator;
        }

        internal IEnumerator enumator;
    }

    #endregion

    #region 异常

    /// <summary>
    /// 循环线程的异常基类
    /// </summary>
    public class LoopThreadException : Exception
    {

        public LoopThreadException() : base()
        {
        }
        public LoopThreadException(string message) : base(message)
        {
        }
        public LoopThreadException(string message, Exception exception) : base(message, exception)
        {
        }

    }

    /// <summary>
    /// 循环线程运行时引发的异常
    /// </summary>
    public class LoopThreadRunException : LoopThreadException
    {
        public LoopThreadRunException() : base("循环线程运行时引发的异常")
        {
        }
        public LoopThreadRunException(string message) : base(message)
        {
        }
        public LoopThreadRunException(string message, Exception exception) : base(message, exception)
        {
        }
    }

    /// <summary>
    /// 循环线程启动时引发的异常
    /// </summary>
    public class LoopStartException : LoopThreadException
    {
        public LoopStartException() : base("循环线程启动时引发的异常")
        {
        }
        public LoopStartException(string message) : base(message)
        {
        }
        public LoopStartException(string message, Exception exception) : base(message, exception)
        {
        }
    }

    #endregion

    #region 计时器

    /// <summary>
    /// 一个循环线程计时器
    /// </summary>
    public sealed class LoopThreadTimer : TickTimeTimer
    {

        /// <summary>
        /// 实例化一个计时器
        /// </summary>
        /// <param name="loop">指定到的循环线程实例</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public LoopThreadTimer(LoopFunction loop)
        {
            p_loop = loop ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// 实例化一个计时器，指定计时器运行时间
        /// </summary>
        /// <param name="loop">指定到的循环线程实例</param>
        /// <param name="span">计时器运行时间</param>
        public LoopThreadTimer(LoopFunction loop, TimeSpan span) : base((ulong)span.Ticks)
        {
            p_loop = loop ?? throw new ArgumentNullException();
        }

        private LoopFunction p_loop;

        protected override DateTime NowTime => p_loop.p_nowTime;

        protected override ulong NowTimeTick => (ulong)p_loop.p_nowTime.Ticks;

        /// <summary>
        /// 获取此计时器的循环线程
        /// </summary>
        /// <returns>此计时器的循环线程</returns>
        public LoopFunction GetLoop() => p_loop;
    }

    /// <summary>
    /// 一个循环线程缩放时间计时器
    /// </summary>
    public sealed class LoopThreadScaleTimer : TickTimeTimer
    {

        /// <summary>
        /// 实例化一个计时器
        /// </summary>
        /// <param name="loop">指定到的循环线程实例</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public LoopThreadScaleTimer(LoopFunction loop)
        {
            p_loop = loop ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// 实例化一个计时器，指定计时器运行时间
        /// </summary>
        /// <param name="loop">指定到的循环线程实例</param>
        /// <param name="span">计时器运行时间</param>
        public LoopThreadScaleTimer(LoopFunction loop, TimeSpan span) : base((ulong)span.Ticks)
        {
            p_loop = loop ?? throw new ArgumentNullException();
        }

        private LoopFunction p_loop;

        protected override DateTime NowTime => p_loop.p_nowScaleTime;

        protected override ulong NowTimeTick => (ulong)p_loop.p_nowScaleTime.Ticks;

        /// <summary>
        /// 获取此计时器的循环线程
        /// </summary>
        /// <returns>此计时器的循环线程</returns>
        public LoopFunction GetLoop() => p_loop;
    }

    #endregion

}
