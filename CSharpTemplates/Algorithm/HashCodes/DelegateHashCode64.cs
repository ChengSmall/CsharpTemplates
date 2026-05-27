using System;

namespace Cheng.Algorithm.HashCodes
{
    /// <summary>
    /// 使用委托函数实例化一个64哈希代码提取器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelegateHashCode64<T> : BaseHashCode64<T>
    {
        /// <summary>
        /// 使用委托函数实例化一个64哈希代码提取器
        /// </summary>
        /// <param name="toHashCode64">指定委托</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DelegateHashCode64(Func<T, long> toHashCode64)
        {
            p_toHashCode64 = toHashCode64 ?? throw new ArgumentNullException();
        }
        private Func<T, long> p_toHashCode64;

        public override long GetHashCode64(T value)
        {
            return p_toHashCode64.Invoke(value);
        }
    }
}
