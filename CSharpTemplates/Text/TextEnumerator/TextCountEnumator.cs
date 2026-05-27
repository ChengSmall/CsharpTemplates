using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Cheng.Texts.NumEnumator
{

    /// <summary>
    /// 数值转化类型
    /// </summary>
    public enum NumFormat : byte
    {
        /// <summary>
        /// 10进制数
        /// </summary>
        D10 = 1,
        /// <summary>
        /// 16进制数
        /// </summary>
        X16 = 2
    }

    /// <summary>
    /// 十进制计数文本枚举器
    /// </summary>
    /// <remarks>
    /// <para>按照十进制数从指定数字开始推进计数，并将每次推进后的数字以文本返回到当前枚举参数</para>
    /// </remarks>
    public sealed class NumEnumatorName : TextEnumerator
    {

        /// <summary>
        /// 实例化十进制计数文本枚举器
        /// </summary>
        /// <param name="format">数值格式</param>
        /// <param name="lastName">前导名称，null表示没有</param>
        /// <param name="nextName">后导名称，null表示没有</param>
        public NumEnumatorName(NumFormat format, string lastName, string nextName) : this(format, lastName, nextName, 0)
        {
        }

        /// <summary>
        /// 实例化十进制计数文本枚举器
        /// </summary>
        /// <param name="format">数值格式</param>
        /// <param name="lastName">前导名称，null表示没有</param>
        /// <param name="nextName">后导名称，null表示没有</param>
        /// <param name="initCount"></param>
        public NumEnumatorName(NumFormat format, string lastName, string nextName, int initCount)
        {
            p_last = lastName;
            p_next = nextName;
            p_name = null;
            p_format = format;
            p_init = initCount - 1;

            p_count = p_init;
        }

        private readonly int p_init;
        private string p_last;
        private string p_name;
        private string p_next;

        private int p_count;
        private NumFormat p_format;

        public override string CurructText => p_name;

        public override bool CanReset => true;

        public override bool MoveNext()
        {
            if (p_count == int.MaxValue) return false;
            p_count++;
            p_name = (p_last + p_count.ToString((p_format == NumFormat.X16) ? "X" : "D") + p_next);
            return true;
        }

        public override void Reset(int maxCount)
        {
            p_count = p_init;
            p_name = null;
        }

        public override void Dispose()
        {
            p_count = int.MaxValue;
            p_name = null;
        }

    }

    /// <summary>
    /// 十进制定长值计数文本枚举器
    /// </summary>
    /// <remarks>
    /// <para>按照十进制数从指定数字开始推进计数，并将每次推进后的数字以文本返回到当前枚举参数，并且每次推进的数值文本都是固定的字符长度</para>
    /// </remarks>
    public sealed class NumFixedCountEnumatorName : TextEnumerator
    {

        /// <summary>
        /// 实例化十进制定长值计数文本枚举器
        /// </summary>
        /// <param name="maxCount">指定一次最大的推进数</param>
        /// <param name="format">指定数值格式</param>
        /// <param name="lastName">前导名称，null表示没有</param>
        /// <param name="nextName">后导名称，null表示没有</param>
        /// <exception cref="ArgumentOutOfRangeException">参数<paramref name="maxCount"/>不大于0</exception>
        public NumFixedCountEnumatorName(int maxCount, NumFormat format, string lastName, string nextName)
        {
            if (maxCount <= 0) throw new ArgumentOutOfRangeException();

            p_last = lastName;
            p_next = nextName;
            p_maxCount = maxCount - 1;
            p_format = format;
            p_count = -1;
            p_maxDigit = ReMaxDigit(p_maxCount, format);
            p_name = null;
            p_initMax = p_maxCount;
        }

        private string p_last;
        private string p_name;
        private string p_next;
        private int p_maxCount;
        private int p_maxDigit;
        private int p_count;
        private int p_initMax;
        private NumFormat p_format;

        public override string CurructText => p_name;

        public override bool CanReset => true;

        public override bool MoveNext()
        {
            if (p_count > p_maxCount)
            {
                p_name = null;
                return false;
            }
            p_count++;

            string numStr;
            if (p_format == NumFormat.D10)
            {
                numStr = p_count.ToString("D" + p_maxDigit.ToString());
            }
            else
            {
                numStr = p_count.ToString("X" + p_maxDigit.ToString());
            }
            p_name = (p_last + numStr + p_next);

            return true;
        }

        public override void Reset(int maxCount)
        {
            if (maxCount < 0) maxCount = p_initMax;
            p_count = -1;
            p_maxCount = maxCount - 1;
            p_name = null;
            p_maxDigit = ReMaxDigit(p_maxCount, p_format);
        }

        /// <summary>
        /// 返回参数<paramref name="value"/>的十进制位数
        /// </summary>
        /// <param name="value">参数</param>
        /// <param name="format">数格式</param>
        /// <returns>参数的所在十进制位数</returns>
        public static int ReMaxDigit(int value, NumFormat format)
        {
            if (format == NumFormat.D10)
            {
                if (value < 10)
                {
                    return 1;
                }
                else if (value < 100)
                {
                    return 2;
                }
                else if (value < 1000)
                {
                    return 3;
                }
                else if (value < 10000)
                {
                    return 4;
                }
                else if (value < 100000)
                {
                    return 5;
                }
                else if (value < 1000000)
                {
                    return 6;
                }
                else if (value < 10000000)
                {
                    return 7;
                }
                else if (value < 100000000)
                {
                    return 8;
                }
                else if (value < 1000000000)
                {
                    return 9;
                }
                return 10;
            }

            if (value < 0x10)
            {
                return 1;
            }
            else if (value < 0x100)
            {
                return 2;
            }
            else if (value < 0x1000)
            {
                return 3;
            }
            else if (value < 0x10000)
            {
                return 4;
            }
            else if (value < 0x100000)
            {
                return 5;
            }
            else if (value < 0x1000000)
            {
                return 6;
            }
            else if (value < 0x10000000)
            {
                return 7;
            }
            return 8;
        }

        public override void Dispose()
        {
            p_count = p_maxCount + 1;
        }

    }

    /// <summary>
    /// 字母计数文本枚举器
    /// </summary>
    /// <remarks>
    /// <para>使用字母计数方式枚举文本，每次将计数器推进后返回对应的固定长度的字母文本</para>
    /// </remarks>
    public sealed unsafe class LetterNumEnumatorName : TextEnumerator
    {

        /// <summary>
        /// 实例化字母计数文本枚举器
        /// </summary>
        /// <param name="maxCount">最大数</param>
        /// <param name="lastName">前置字符串，null表示没有</param>
        /// <param name="nextName">后置字符串，null表示没有</param>
        /// <param name="letterUpper">字母是否大写</param>
        public LetterNumEnumatorName(int maxCount, string lastName, string nextName, bool letterUpper)
        {
            if (maxCount < 0) maxCount = int.MaxValue;
            p_name = null;
            p_maxCount = maxCount - 1;
            p_last = lastName;
            p_next = nextName;
            p_count = -1;
            p_big = letterUpper;
        }

        private string p_last;
        private string p_name;
        private string p_next;
        private int p_maxCount;
        private int p_count;
        private bool p_big;

        public override string CurructText => p_name;

        public override bool CanReset => true;

        /// <summary>
        /// 返回26进制位数
        /// </summary>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        static int reDigit(int maxCount)
        {
            int count = 0;

            while (maxCount != 0)
            {
                maxCount /= 26;
                count++;
            }

            return count;
        }

        /// <summary>
        /// 返回字母数
        /// </summary>
        /// <param name="value">字母数的值</param>
        /// <param name="maxCount">最高值</param>
        /// <param name="big">开启大写</param>
        /// <returns></returns>
        static string toLetter(int value, int maxCount, bool big)
        {
            //字母位数
            var digitCount = reDigit(maxCount);
            //缓存
            char* cs = stackalloc char[digitCount];

            char firstChar;

            if (big) firstChar = 'A';
            else firstChar = 'a';

            int i;

            for (i = digitCount - 1; (value != 0) && (i >= 0); i--)
            {
                //获取单个位数的26进制值
                var nd = value % 26;

                cs[i] = (char)(firstChar + nd);

                value /= 26;
            }

            for (; i >= 0; i--)
            {
                cs[i] = firstChar;
            }

            return new string(cs, 0, digitCount);
        }

        public override bool MoveNext()
        {
            if (p_count > p_maxCount)
            {
                p_name = null;
                return false;
            }

            p_count++;

            p_name = (p_last + toLetter(p_count, p_maxCount, p_big) + p_next);

            return true;
        }

        public override void Reset(int maxCount)
        {
            if (maxCount < 0) maxCount = int.MaxValue;
            p_name = null;
            p_maxCount = maxCount - 1;
            p_count = -1;
        }

        public override void Dispose()
        {
            p_count = p_maxCount + 1;
            p_name = null;
        }

    }

}
