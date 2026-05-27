using System;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Algorithm.Sorts.Comparers
{

    /// <summary>
    /// 数字分离式依次比较字符的字符串比较器
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该比较器比较时从第一个索引向后依次比较字符，并返回比较结果；<br/>
    /// 当遇到连续的数字字符时，则会将其转化成值比较；
    /// </para>
    /// <para>此类对象内有缓冲区，因此使用多线程调用时请做好线程安全措施</para>
    /// </remarks>
    public sealed class StrNumSeparationComparer : Comparer<string>
    {

        #region 构造

        /// <summary>
        /// 使用默认比较器初始化
        /// </summary>
        public StrNumSeparationComparer()
        {
            charComparator = Comparer<char>.Default;
            numComparator = Comparer<long>.Default;
        }

        /// <summary>
        /// 使用自定义值比较器初始化
        /// </summary>
        /// <param name="numComp">指定值比较器，使其比较数字时，使用自定义比较器比较，null表示默认比较器</param>
        public StrNumSeparationComparer(IComparer<long> numComp)
        {
            charComparator = Comparer<char>.Default;
            numComparator = numComp ?? Comparer<long>.Default;
        }

        /// <summary>
        /// 使用自定义字符比较器初始化
        /// </summary>
        /// <param name="charComp">指定字符比较器，使其比较字符时，使用自定义比较器比较，null表示默认比较器</param>
        public StrNumSeparationComparer(IComparer<char> charComp)
        {
            charComparator = charComp ?? Comparer<char>.Default;
            numComparator = Comparer<long>.Default;
        }

        /// <summary>
        /// 使用自定义的字符和值比较器初始化
        /// </summary>
        /// <param name="charComp">指定字符比较器，使其比较字符时，使用自定义比较器比较，null表示默认比较器</param>
        /// <param name="numComp">指定值比较器，使其比较数字时，使用自定义比较器比较，null表示默认比较器</param>
        public StrNumSeparationComparer(IComparer<char> charComp, IComparer<long> numComp)
        {
            charComparator = charComp ?? Comparer<char>.Default;
            numComparator = numComp ?? Comparer<long>.Default;
        }

        #endregion

        #region 参数

        private IComparer<char> charComparator;
        private IComparer<long> numComparator;
        private StringBuilder p_buffer1 = new StringBuilder(2);
        private StringBuilder p_buffer2 = new StringBuilder(2);

        #endregion

        #region 功能

        /// <summary>
        /// 访问或设置数值比较器
        /// </summary>
        /// <exception cref="ArgumentNullException">比较器设置为null</exception>
        public IComparer<long> NumComparator
        {
            get => numComparator;
            set
            {
                numComparator = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 访问或设置字符比较器
        /// </summary>
        /// <exception cref="ArgumentNullException">比较器设置为null</exception>
        public IComparer<char> CharComparator
        {
            get => charComparator;
            set
            {
                charComparator = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 清理当前实例的缓冲区（已做线程安全封装）
        /// </summary>
        public void ClearBuffer()
        {          
            lock (p_buffer1)
            {
                lock (p_buffer2)
                {
                    p_buffer1.Length = 0;
                    p_buffer2.Length = 0;
                    p_buffer1.Capacity = 2;
                    p_buffer2.Capacity = 2;
                }
            }
           
        }

        #endregion

        public sealed override int Compare(string x, string y)
        {
            if ((object)x == (object)y) return 0;

            if (x is null || y is null)
            {
                return x == null ? int.MinValue : (y == null) ? 0 : int.MaxValue;
            }
            int compOver;
            long leftnum, rightnum;

            char c1, c2;
            int charCompNum;
            StringBuilder s1 = p_buffer1, s2 = p_buffer2;
            int i = 0, j = 0;
 
            int leftSize = x.Length, rightSize = y.Length;

            //逐字符处理
            while (i < leftSize && j < rightSize)
            {
                bool b = char.IsDigit(x[i]) && char.IsDigit(y[j]);
                if (b)
                {
                    s1.Length = 0;
                    s2.Length = 0;

                    while (i < leftSize && char.IsDigit(x[i]))
                    {
                        s1.Append(x[i]);
                        i++;
                    }
                    while (j < rightSize && char.IsDigit(y[j]))
                    {
                        s2.Append(y[j]);
                        j++;
                    }

                    //比较数字
                    
                    if ((!long.TryParse(s1.ToString(), out leftnum)) || (!long.TryParse(s2.ToString(), out rightnum)))
                    {
                        //goto TryJump;
                        continue;
                    }

                    //leftnum = long.Parse(s1.ToString());
                    //rightnum = long.Parse(s2.ToString());
                       
                    compOver = numComparator.Compare(leftnum, rightnum);

                    if (compOver != 0)
                    {
                        return compOver;
                    }

                    //比较数字
                    
                }
                else
                {

                    c1 = x[i]; c2 = y[i];

                    charCompNum = charComparator.Compare(c1, c2);

                    if (charCompNum != 0)
                    {
                        return charCompNum;
                    }

                    i++;
                    j++;

                }

            }

            //if (leftSize == rightSize)
            //{
            //    return 0;
            //}
            //else
            //{
            //    return leftSize < rightSize ? -1 : 1;
            //}

            return leftSize.CompareTo(rightSize);

        }

    }

    /// <summary>
    /// 数字分离式比较字符串的字符串比较器
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该比较器比较时，当遇到连续的数字字符时，会将其转化成值比较<br/>
    /// 除此之外的字符，当作字符串比较
    /// </para>
    /// </remarks>
    public sealed class StrFullNumSepComparer : Comparer<string>
    {

        #region 构造

        /// <summary>
        /// 实例化数字分离字符串比较器
        /// </summary>
        public StrFullNumSepComparer() : this(null, null)
        {
        }

        /// <summary>
        /// 实例化数字分离字符串比较器
        /// </summary>
        /// <param name="stringComparator">非数字的字符串比较器，null表示使用默认实现的比较器</param>
        public StrFullNumSepComparer(IComparer<string> stringComparator) : this(null, stringComparator)
        {
        }

        /// <summary>
        /// 实例化数字分离字符串比较器
        /// </summary>
        /// <param name="numComparator">将字符串转化为数字时的数值比较器，null表示使用默认实现的比较器</param>
        public StrFullNumSepComparer(IComparer<long> numComparator) : this(numComparator, null)
        {
        }

        /// <summary>
        /// 实例化数字分离字符串比较器
        /// </summary>
        /// <param name="numComparator">将字符串转化为数字时的数值比较器，null表示使用默认实现的比较器</param>
        /// <param name="stringComparator">非数字的字符串比较器，null表示使用默认实现的比较器</param>
        public StrFullNumSepComparer(IComparer<long> numComparator, IComparer<string> stringComparator)
        {
            this.numComparator = numComparator ?? Comparer<long>.Default;
            this.strComparator = stringComparator ?? Comparer<string>.Default;
        }

        #endregion

        #region 参数

        private IComparer<string> strComparator;

        private IComparer<long> numComparator;

        private StringBuilder p_buffer1 = new StringBuilder();

        private StringBuilder p_buffer2 = new StringBuilder();

        #endregion

        #region 功能

        /// <summary>
        /// 访问或设置数值比较器
        /// </summary>
        /// <exception cref="ArgumentNullException">比较器设置为null</exception>
        public IComparer<long> NumComparator
        {
            get => numComparator;
            set
            {
                numComparator = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 访问或设置字符串比较器
        /// </summary>
        /// <exception cref="ArgumentNullException">比较器设置为null</exception>
        public IComparer<string> StringComparator
        {
            get => strComparator;
            set
            {
                strComparator = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 清理当前实例的缓冲区
        /// </summary>
        public void ClearBuffer()
        {
            p_buffer1.Clear();
            p_buffer2.Clear();
            if(p_buffer1.Capacity > 2) p_buffer1.Capacity = 2;
            if (p_buffer2.Capacity > 2) p_buffer2.Capacity = 2;
        }

        #endregion

        public sealed override int Compare(string x, string y)
        {
            if ((object)x == (object)y) return 0;

            if (x is null || y is null)
            {
                return x == null ? int.MinValue : (y == null) ? 0 : int.MaxValue;
            }
            int compOver;
            long leftnum, rightnum;

            //char c1, c2;
            int charCompNum;
            StringBuilder s1 = p_buffer1, s2 = p_buffer2;
            int i = 0, j = 0;

            int leftSize = x.Length, rightSize = y.Length;

            //逐字符处理
            while (i < leftSize && j < rightSize)
            {
                bool b = char.IsDigit(x[i]) && char.IsDigit(y[j]);
                if (b)
                {
                    s1.Clear();
                    s2.Clear();

                    while (i < leftSize && char.IsDigit(x[i]))
                    {
                        s1.Append(x[i]);
                        i++;
                    }
                    while (j < rightSize && char.IsDigit(y[j]))
                    {
                        s2.Append(y[j]);
                        j++;
                    }

                    //比较数字
                    var s1str = s1.ToString();
                    var s2str = s2.ToString();
                    if ((!long.TryParse(s1str, out leftnum)) || (!long.TryParse(s2str, out rightnum)))
                    {
                        //goto TryJump;
                        compOver = strComparator.Compare(s1str, s2str);
                        if (compOver != 0)
                        {
                            return compOver;
                        }
                        continue;
                    }

                    //leftnum = long.Parse(s1.ToString());
                    //rightnum = long.Parse(s2.ToString());

                    compOver = numComparator.Compare(leftnum, rightnum);
                    if (compOver != 0)
                    {
                        return compOver;
                    }
                    continue;
                }
                else
                {
                    s1.Clear();
                    s2.Clear();
                    while (i < leftSize && (!char.IsDigit(x[i])))
                    {
                        s1.Append(x[i]);
                        i++;
                    }
                    while (j < rightSize && (!char.IsDigit(y[j])))
                    {
                        s2.Append(y[j]);
                        j++;
                    }

                    //c1 = x[i]; c2 = y[i];
                    charCompNum = strComparator.Compare(s1.ToString(), s2.ToString());

                    if (charCompNum != 0)
                    {
                        return charCompNum;
                    }

                    //i++;
                    //j++;
                }

            }

            return leftSize.CompareTo(rightSize);

        }
    }

}
