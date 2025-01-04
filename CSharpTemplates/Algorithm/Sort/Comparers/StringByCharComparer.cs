using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;

namespace Cheng.Algorithm.Sorts.Comparers
{

    /// <summary>
    /// 指定字符比较器依次比较字符的字符串比较器
    /// </summary>
    public class StringByCharComparer : Comparer<string>
    {

        #region 构造

        /// <summary>
        /// 实例化字符串比较器
        /// </summary>
        /// <param name="comparer">自定义字符比较器，null表示默认比较器</param>
        public StringByCharComparer(IComparer<char> comparer)
        {
            if (comparer is null) p_comparer = Comparer<char>.Default;
            else p_comparer = comparer; 
        }

        /// <summary>
        /// 实例化字符串比较器，使用默认字符比较器
        /// </summary>
        public StringByCharComparer()
        {
            p_comparer = Comparer<char>.Default;
        }

        #endregion

        #region 参数

        private IComparer<char> p_comparer;

        /// <summary>
        /// 访问或设置内部字符比较器
        /// </summary>
        /// <value>要设置的字符比较强，设为null则会设为默认实现的比较器</value>
        public IComparer<char> Comparer
        {
            get => p_comparer;
            set
            {
                //if (value is null) throw new ArgumentNullException();
                lock (p_comparer)
                {
                    p_comparer = value ?? Comparer<char>.Default;
                }
            }
        }

        #endregion

        public sealed override int Compare(string x, string y)
        {
            if ((object)x == (object)y) return 0;
            if (x is null || y is null)
            {
                return (x is null) ? -1 : (y is null ? 0 : 1);
            }

            int length;

            //三项 null长度相等，false表示x长，true表示y长
            bool? isYLong;

            //获取最短长度和长短判断
            if(x.Length < y.Length)
            {
                length = x.Length;
                isYLong = true;
            }
            else if(x.Length > y.Length)
            {
                length = y.Length;
                isYLong = false;
            }
            else
            {
                //长度相等
                length = x.Length;
                if (length == 0) return 0;              
                isYLong = null;
            }

            //开始比较
            int i;

            int r;
            for (i = 0; i < length; i++)
            {
                r = p_comparer.Compare(x[i], y[i]);
                if (r != 0) return r;
            }

            //全等
            if (!isYLong.HasValue) return 0;

            return isYLong.Value ? -1 : 1;

        }

    }
}
