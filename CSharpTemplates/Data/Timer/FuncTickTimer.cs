using System;


namespace Cheng.Timers
{

    /// <summary>
    /// 使用返回当前时间刻的委托进行计时的计时器
    /// </summary>
    public sealed class FuncTickTimer : TickTimeTimer
    {

        /// <summary>
        /// 实例化委托计时器
        /// </summary>
        /// <param name="getTickFunc">表示一个返回当前时间刻的委托函数</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public FuncTickTimer(Func<long> getTickFunc)
        {
            p_getTickFunc = getTickFunc ?? throw new ArgumentNullException();
        }

        private readonly Func<long> p_getTickFunc;

        protected override ulong NowTimeTick => (ulong)p_getTickFunc.Invoke();

        protected override DateTime NowTime => new DateTime((long)p_getTickFunc.Invoke());

    }

}