using System;
using System.Collections.Generic;

namespace Cheng.Algorithm.Sorts.Comparers
{

    /// <summary>
    /// 允许反转比较结果的比较器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class InvertComparer<T> : Comparer<T>
    {

        /// <summary>
        /// 实例化一个反转比较结果的比较器
        /// </summary>
        /// <param name="comparer">要反转的比较器</param>
        public InvertComparer(IComparer<T> comparer)
        {
            this.comparer = comparer ?? throw new ArgumentNullException();
            p_isInvert = true;
        }

        /// <summary>
        /// 实例化一个可反转比较结果的比较器
        /// </summary>
        /// <param name="comparer">要反转的比较器</param>
        /// <param name="invert">是否使用反转功能</param>
        public InvertComparer(IComparer<T> comparer, bool invert)
        {
            this.comparer = comparer ?? throw new ArgumentNullException();
            p_isInvert = invert;
        }

        private IComparer<T> comparer;

        private bool p_isInvert;

        /// <summary>
        /// 访问或设置当前比较器是否反转；默认为true
        /// </summary>
        public bool IsInvert
        {
            get => p_isInvert;
            set
            {                
                p_isInvert = value;                                
            }
        }

        public sealed override int Compare(T x, T y)
        {
            var re = comparer.Compare(x, y);
            return p_isInvert ? (-re) : re;
        }
    }

}
