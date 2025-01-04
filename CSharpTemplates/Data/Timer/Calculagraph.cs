using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Timers
{

    /// <summary>
    /// 一个通用计时器，可以用做一些测量时间的工作
    /// </summary>
    /// <remarks>若要更加精确的测量计算机性能，请使用<see cref="System.Diagnostics.Stopwatch"/></remarks>
    public sealed class Calculagraph : TickTimeTimer
    {

        /// <summary>
        /// 实例化一个计时器
        /// </summary>
        public Calculagraph() { }

        /// <summary>
        /// 实例化一个计时器并指定计时器运行时间
        /// </summary>
        /// <param name="span">运行时间</param>
        public Calculagraph(TimeSpan span) : base((ulong)span.Ticks)
        {
        }

        protected override DateTime NowTime => DateTime.UtcNow;
        protected override ulong NowTimeTick => (ulong)DateTime.UtcNow.Ticks;

        /// <summary>
        /// 返回一个开始计时的新实例
        /// </summary>
        /// <returns>已经开始计时的新实例</returns>
        public static Calculagraph CreateRunTime()
        {
            Calculagraph t = new Calculagraph();
            t.Start();
            return t;
        }

    }

}
