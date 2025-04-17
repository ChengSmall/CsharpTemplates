using System;

namespace Cheng.Timers
{

    /// <summary>
    /// 计时器公共接口
    /// </summary>
    public interface ITimer
    {

        /// <summary>
        /// 获取当前计时器的运行时间
        /// </summary>
        TimeSpan Elapsed { get; }
        /// <summary>
        /// 当前计时器是否正在运行
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// 开启计时器计时
        /// </summary>
        void Start();
        /// <summary>
        /// 停止计时器计时
        /// </summary>
        void Stop();
        /// <summary>
        /// 停止并重置计时器运行时间
        /// </summary>
        void Reset();
        /// <summary>
        /// 清空计时器运行时间并重新开始计时
        /// </summary>
        void Restart();
        /// <summary>
        /// 重置计时器的时间记录，且不影响计时器运行
        /// </summary>
        void Clear();
        /// <summary>
        /// 将计时器的时间写入到缓冲区参数
        /// </summary>
        void Flush();

        /// <summary>
        /// 为计时器添加计时器时间
        /// </summary>
        /// <param name="span">添加的时间</param>
        void AddElapsed(TimeSpan span);

        /// <summary>
        /// 为计时器减少时间
        /// </summary>
        /// <param name="span">要减少的时间</param>
        void SubElapsed(TimeSpan span);
    }

    /// <summary>
    /// 一个以Tick结构作为时间参数的计时器
    /// </summary>
    public abstract class TickTimeTimer : ITimer
    {

        #region 构造

        protected TickTimeTimer()
        {
            p_running = false;
            p_elapsed = 0L;
        }

        /// <summary>
        /// 初始化计时器并定义计时器的运行时间
        /// </summary>
        /// <param name="elapsed">定义计时器的运行时间</param>
        protected TickTimeTimer(ulong elapsed)
        {
            p_running = false;
            p_elapsed = elapsed;
        }

        #endregion

        #region 参数

        private bool p_running;

        private ulong p_elapsed;

        private ulong p_startTime;

        #endregion

        #region 功能

        /// <summary>
        /// 获取当前的时间刻
        /// </summary>
        /// <returns>在派生类重写此属性，以返回当前时间刻</returns>
        protected virtual DateTime NowTime
        {
            get => new DateTime((long)NowTimeTick);
        }

        /// <summary>
        /// 获取当前的时间刻tick
        /// </summary>
        /// <returns>在派生类重写此属性，以返回当前时间刻</returns>
        protected abstract ulong NowTimeTick { get; }

        /// <summary>
        /// 获取当前计时器的运行时间
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                if (p_running) return new TimeSpan((long)(p_elapsed + (NowTimeTick - p_startTime)));
                return new TimeSpan((long)p_elapsed);
            }
        }

        /// <summary>
        /// 获取当前计时器的运行时间
        /// </summary>
        public ulong ElapsedTick
        {
            get
            {
                if (p_running) return (p_elapsed + (NowTimeTick - p_startTime));
                return p_elapsed;
            }
        }

        /// <summary>
        /// 当前计时器是否正在运行
        /// </summary>
        public bool IsRunning => p_running;

        /// <summary>
        /// 获取当前时间缓冲参数
        /// </summary>
        public ulong BufferElapsed
        {
            get => p_elapsed;
        }

        /// <summary>
        /// 开启计时器计时
        /// </summary>
        public void Start()
        {
            if(!p_running)
            {
                p_startTime = NowTimeTick;
                p_running = true;
            }
        }

        /// <summary>
        /// 停止计时器计时
        /// </summary>
        public void Stop()
        {
            if (p_running)
            {
                p_elapsed += (ulong)(NowTimeTick - p_startTime);
                p_running = false;
            }
        }

        /// <summary>
        /// 停止计时器并重置计时器的运行时间
        /// </summary>
        public void Reset()
        {
            p_elapsed = 0;
            p_running = false;
        }

        /// <summary>
        /// 清空计时器运行时间并重新开始计时
        /// </summary>
        public void Restart()
        {
            p_elapsed = 0;
            p_startTime = NowTimeTick;
            p_running = true;
        }

        /// <summary>
        /// 重置计时器的时间记录，且不影响计时器运行
        /// </summary>
        public void Clear()
        {
            p_elapsed = 0;
            if(p_running) p_startTime = NowTimeTick;
        }

        /// <summary>
        /// 将计时器的时间写入到缓冲区参数
        /// </summary>
        public void Flush()
        {
            if (p_running)
            {
                var tick = NowTimeTick;
                p_elapsed += (ulong)(tick - p_startTime);
                p_startTime = tick;
            }
        }

        /// <summary>
        /// 为计时器添加计时器时间
        /// </summary>
        /// <param name="span">添加的时间</param>
        public void AddElapsed(ulong span)
        {
            p_elapsed += span;
        }

        /// <summary>
        /// 为计时器添加计时器时间
        /// </summary>
        /// <param name="span">添加的时间</param>
        public void AddElapsed(TimeSpan span)
        {
            AddElapsed((ulong)span.Ticks);
        }

        /// <summary>
        /// 为计时器减少时间
        /// </summary>
        /// <param name="span">要减少的时间</param>
        public void SubElapsed(ulong span)
        {

            if (p_elapsed > span)
            {
                p_elapsed -= span;
            }
            else
            {
                p_elapsed = 0;
            }
            
        }

        /// <summary>
        /// 为计时器减少时间
        /// </summary>
        /// <param name="span">要减少的时间</param>
        public void SubElapsed(TimeSpan span)
        {
            SubElapsed((ulong)span.Ticks);
        }

        #endregion

    }

    /// <summary>
    /// 一个以小数作为时间参数的计时器
    /// </summary>
    public abstract class SingleTimer : ITimer
    {

        #region 构造

        protected SingleTimer()
        {
            p_running = false;
            p_elapsed = 0d;
        }

        /// <summary>
        /// 初始化计时器并定义计时器的运行时间
        /// </summary>
        /// <param name="elapsed">定义计时器的运行时间</param>
        protected SingleTimer(double elapsed)
        {
            p_running = false;
            p_elapsed = elapsed;
        }

        #endregion

        #region 参数

        private bool p_running;

        private double p_elapsed;

        private double p_startTime;

        #endregion

        #region 功能

        /// <summary>
        /// 获取当前的时间刻
        /// </summary>
        /// <returns>在派生类重写此属性，以返回当前时间刻</returns>
        protected abstract double NowTime { get; }

        /// <summary>
        /// 获取当前计时器的运行时间
        /// </summary>
        public double Elapsed
        {
            get
            {
                if (p_running) return p_elapsed + (NowTime - p_startTime);
                return p_elapsed;
            }
        }

        /// <summary>
        /// 在派生类重写，将浮点型时间转化为<see cref="TimeSpan"/>以实现公共接口
        /// </summary>
        /// <param name="elapsed"><see cref="Elapsed"/>属性的浮点刻度值</param>
        /// <returns>一个与<see cref="Elapsed"/>属性等价的<see cref="TimeSpan"/>实例</returns>
        protected abstract TimeSpan SingleTimeToTimeSpan(double elapsed);

        /// <summary>
        /// 在派生类重写，将<see cref="TimeSpan"/>时间转化为浮点型以实现公共接口
        /// </summary>
        /// <param name="span">时间间隔</param>
        /// <returns>与<paramref name="span"/>对应的浮点型时间间隔</returns>
        protected abstract double TimeSpanToDouble(TimeSpan span);

        /// <summary>
        /// 当前计时器是否正在运行
        /// </summary>
        public bool IsRunning => p_running;

        TimeSpan ITimer.Elapsed
        {
            get => SingleTimeToTimeSpan(Elapsed);
        }

        /// <summary>
        /// 获取当前时间缓冲参数
        /// </summary>
        public double BufferElapsed
        {
            get => p_elapsed;
        }

        /// <summary>
        /// 开启计时器计时
        /// </summary>
        public void Start()
        {
            if (!p_running)
            {
                p_startTime = NowTime;
                p_running = true;
            }
        }

        /// <summary>
        /// 停止计时器计时
        /// </summary>
        public void Stop()
        {
            if (p_running)
            {
                p_elapsed += NowTime - p_startTime;
                p_running = false;
                if (p_elapsed < 0d) p_elapsed = 0d;
            }
        }

        /// <summary>
        /// 停止计时器并重置计时器的运行时间
        /// </summary>
        public void Reset()
        {
            p_elapsed = 0d;
            p_running = false;
        }

        /// <summary>
        /// 清空计时器运行时间并重新开始计时
        /// </summary>
        public void Restart()
        {
            p_elapsed = 0d;
            p_startTime = NowTime;
            p_running = true;
        }

        /// <summary>
        /// 重置计时器的时间记录，且不影响计时器运行
        /// </summary>
        public void Clear()
        {
            p_elapsed = 0;
            if (p_running) p_startTime = NowTime;
        }

        /// <summary>
        /// 将计时器的时间写入到缓冲区参数
        /// </summary>
        public void Flush()
        {
            if (p_running)
            {
                var tick = NowTime;
                p_elapsed += tick - p_startTime;
                p_startTime = tick;
            }
        }

        /// <summary>
        /// 为计时器添加计时器时间
        /// </summary>
        /// <param name="span">添加的时间</param>
        public void AddElapsed(double span)
        {
            p_elapsed += span;
        }

        void ITimer.AddElapsed(TimeSpan span)
        {
            AddElapsed(TimeSpanToDouble(span));
        }

        /// <summary>
        /// 为计时器减少时间
        /// </summary>
        /// <param name="span">要减少的时间</param>
        public void SubElapsed(double span)
        {
            p_elapsed -= span;
        }

        void ITimer.SubElapsed(TimeSpan span)
        {
            SubElapsed(TimeSpanToDouble(span));
        }
        #endregion

    }

}
