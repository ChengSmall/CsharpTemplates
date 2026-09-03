using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Cheng.Memorys;

using FPT = Cheng.DataStructure.FixedPoints.FixedPoint32I22P10;
using FPRAW = System.UInt32;

namespace Cheng.DataStructure.FixedPoints
{

    /// <summary>
    /// 32位定点数 - 整数位22bit 小数位10bit
    /// </summary>
    /// <remarks>
    /// <para>
    /// 32位定点数，采用4字节整数最为基本数据，前10bit是小数位，剩余22bit是整数位，共同组合成为定点小数<br/>
    /// 位结构如下所示，使用G作为整数bit，X作为小数bit，N作为符号位:<br/>
    /// <code>NGGGGGGG GGGGGGGG GGGGGG.XX XXXXXXXX</code>
    /// </para>
    /// <para>定点数的运算结果具有严格的跨平台一致性</para>
    /// </remarks>
    public readonly unsafe struct FixedPoint32I22P10 : IEquatable<FPT>, IComparable<FPT>
    {

        #region 初始化

        /// <summary>
        /// 使用32位原始值初始化定点数
        /// </summary>
        /// <param name="raw">32位原始值整数</param>
        public FixedPoint32I22P10(FPRAW raw)
        {
            this.raw = raw;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 32位定点数原始数据
        /// </summary>
        public readonly FPRAW raw;

        #endregion

        #region 功能

        #region 常量

#if DEBUG
        /// <summary>
        /// 小数位数
        /// </summary>
#endif
        private const int FractionalBits = 10;

#if DEBUG
        /// <summary>
        /// 缩放因子 1024
        /// </summary>
#endif
        private const int Scale = 1 << FractionalBits;

#if DEBUG
        /// <summary>
        /// ln(2) ≈ 0.6931，Q10格式下约为 0.69314718 * 1024 ≈ 710
        /// </summary>
#endif
        private const int Ln2Q10 = 710;

        /// <summary>
        /// 表示32位定点数最大值的原始整数
        /// </summary>
        public const FPRAW MaxRaw = 0b01111111_11111111_11111111_11111111;

        /// <summary>
        /// 表示32位定点数最小值的原始整数
        /// </summary>
        public const FPRAW MinRaw = ~MaxRaw;

        /// <summary>
        /// 32位定点数最接近0的绝对值的原始整数
        /// </summary>
        public const FPRAW EpsilonRaw = 1;

        /// <summary>
        /// 定点数的最大有效值
        /// </summary>
        public static FPT MaxValue
        {
            get => new FPT(MaxRaw);
        }

        /// <summary>
        /// 定点数的最小有效值
        /// </summary>
        public static FPT MinValue
        {
            get => new FPT(MinRaw);
        }

        /// <summary>
        /// 表示0的定点数
        /// </summary>
        public static FPT Zero => new FPT(0);

        /// <summary>
        /// 大于0的最小值
        /// </summary>
        public static FPT Epsilon
        {
            get => new FPT(EpsilonRaw);
        }

        #endregion

        #region 参数获取

        /// <summary>
        /// 当前值是否是负数
        /// </summary>
        /// <value>返回true表示当前值小于0，符号位是1；返回false表示当前值大于0，位符号位是0</value>
        public bool IsNeg
        {
            get
            {
                return ((int)raw) < 0;
            }
        }

        /// <summary>
        /// 当前值是否等于0
        /// </summary>
        public bool IsZero
        {
            get
            {
                return ((int)raw) == 0;
            }
        }

        /// <summary>
        /// 获取小数位的原始整数
        /// </summary>
        public int FractionalRaw
        {
            get => (int)(raw & 0b11_11111111U);
        }

        /// <summary>
        /// 获取整数位的原始整数
        /// </summary>
        public int IntegerRaw
        {
            get => ((int)raw) >> 10;
        }

        #endregion

        #region 四则运算

        /// <summary>
        /// 计算两数相加之和
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static FPT Add(FPT left, FPT right)
        {
            return new FPT(left.raw + right.raw);
        }

        /// <summary>
        /// 计算两数相减之差
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static FPT Sub(FPT left, FPT right)
        {
            return new FPT(left.raw - right.raw);
        }

        /// <summary>
        /// 乘法运算
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static FPT Mult(FPT left, FPT right)
        {
            return new FPT((FPRAW)((((long)(int)left.raw) * ((long)(int)right.raw)) >> FractionalBits));
        }

        /// <summary>
        /// 除法运算
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>返回<paramref name="left"/><![CDATA[/]]><paramref name="right"/>的商最接近的值</returns>
        /// <exception cref="DivideByZeroException">除0行为</exception>
        public static FPT Div(FPT left, FPT right)
        {
            if (right.IsZero)
            {
                throw new DivideByZeroException();
            }
            return new FPT((uint)((int)((((long)(int)left.raw) << FractionalBits) / ((int)right.raw))));
        }

        #endregion

        #region 高级运算

        #region 辅助封装

#if DEBUG
        /// <summary>
        /// 32位无符号整数平方根（牛顿迭代法）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
#endif
        private static ulong IntegerSqrt(ulong value)
        {
            if (value == 0) return 0;
            var x = value;
            ulong y = 1;
            while (x > y)
            {
                x = (x + y) / 2;
                y = value / x;
            }
            return x;
        }

#if DEBUG
        /// <summary>
        /// 自然对数 ln(x)（纯定点数实现）
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
#endif
        private static FPT f_naturalLog(FPT x)
        {
            // 范围缩减：将x归一化到 [1, 2) 区间，提升泰勒展开精度与速度
            int exponent = 0;
            uint mantissa = x.raw;

            while (mantissa >= 2 * Scale)
            {
                mantissa >>= 1;
                exponent++;
            }
            while (mantissa < Scale)
            {
                mantissa <<= 1;
                exponent--;
            }

            // 计算 t = (m - 1) / (m + 1)
            int t = (((int)(mantissa - Scale)) << FractionalBits) /
                ((int)(mantissa + Scale));

            // 泰勒展开：ln(m) = 2*(t + t^3/3 + t^5/5 + t^7/7 + t^9/9)
            int tSquared = (t * t) >> FractionalBits;
            int term = t;
            int lnMantissa = term;

            term = (term * tSquared) >> FractionalBits;
            lnMantissa += term / 3;

            term = (term * tSquared) >> FractionalBits;
            lnMantissa += term / 5;

            term = (term * tSquared) >> FractionalBits;
            lnMantissa += term / 7;

            term = (term * tSquared) >> FractionalBits;
            lnMantissa += term / 9;

            lnMantissa *= 2;
            // 还原指数部分：ln(x) = ln(m) + exponent * ln(2)
            return new FPT((FPRAW)(lnMantissa + exponent * Ln2Q10));
        }

#if DEBUG
        /// <summary>
        /// 自然指数 e^x（纯定点数实现）
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
#endif
        private static FPT f_exp(FPT x)
        {
            bool isNegative = x.IsNeg;
            uint xAbsolute = isNegative ? (uint)(-(int)x.raw) : x.raw;

            // 范围缩减：x = k*ln2 + r，其中 r ∈ [0, ln2)
            int k = (int)xAbsolute / Ln2Q10;
            int remainder = (int)xAbsolute - k * Ln2Q10;

            // 泰勒展开计算 e^r
            int expR = Scale; // 初始项 1.0
            int term = Scale;

            term = (term * remainder) >> FractionalBits;
            expR += term;

            term = (term * remainder) >> FractionalBits;
            expR += term / 2;

            term = (term * remainder) >> FractionalBits;
            expR += term / 6;

            term = (term * remainder) >> FractionalBits;
            expR += term / 24;

            term = (term * remainder) >> FractionalBits;
            expR += term / 120;

            term = (term * remainder) >> FractionalBits;
            expR += term / 720;

            // 还原指数：e^x = e^r * 2^k
            uint result = k >= 0 ? (uint)expR << k : (uint)(expR >> -k);

            // 负数指数取倒数
            if (isNegative)
            {
                result = Div(new FPT(Scale), new FPT(result)).raw;
            }
            return new FPT(result);
        }

        #endregion

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="num"></param>
        /// <returns><paramref name="num"/>的绝对值</returns>
        public static FPT Abs(FPT num)
        {
            int value = (int)num.raw;
            if (value < 0)
            {
                if (value == int.MinValue) return num;
                value = -value;
            }
            return new FPT((FPRAW)value);
        }

        /// <summary>
        /// 幂运算
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>返回 <paramref name="x"/>的<paramref name="y"/>次方的值</returns>
        /// <exception cref="ArgumentException"><paramref name="x"/>小于或等于0</exception>
        public static FPT Pow(FPT x, FPT y)
        {
            if (x.IsNeg || x.IsZero)
            {
                throw new ArgumentException();
            }

            var exponent = Mult(y, f_naturalLog(x));
            return f_exp(exponent);
        }

        /// <summary>
        /// 计算值的平方根
        /// </summary>
        /// <param name="num"></param>
        /// <returns>返回值的平方根</returns>
        /// <exception cref="ArgumentException">值小于0</exception>
        public static FPT Sqrt(FPT num)
        {
            if (num.IsNeg)
            {
                throw new ArgumentException();
            }
            if (num.IsZero) return num;
            // 设结果为Q10格式，则 result_raw = sqrt(raw * 2^10)
            ulong value = (ulong)num.raw << FractionalBits;
            uint sqrtRaw = (uint)IntegerSqrt(value);
            return new FPT(sqrtRaw);
        }

        /// <summary>
        /// 自然指数
        /// </summary>
        /// <param name="num"></param>
        /// <returns>e的<paramref name="d"/>次幂</returns>
        public static FPT Exp(FPT d)
        {
            return f_exp(d);
        }

        #endregion

        #region 其它运算

        /// <summary>
        /// 将当前值添加一个最小单位步长
        /// </summary>
        /// <returns>运算后的值</returns>
        public FPT AddOnceRaw()
        {
            return new FPT(raw + EpsilonRaw);
        }

        #endregion

        #region 字符串转换

        private unsafe static FPT f_createFromStrBuf(char* str, int length)
        {
            int pos = 0;
            int sign = 1;
            long integerPart = 0;
            long fractionalPart = 0;
            int fractionalDigits = 0;

            while (pos < length && char.IsWhiteSpace(str[pos])) pos++;

            // 处理正负符号位
            if (pos < length)
            {
                if (str[pos] == '-')
                {
                    sign = -1;
                    pos++;
                }
                else if (str[pos] == '+')
                {
                    pos++;
                }
            }

            // 解析整数部分
            while (pos < length && char.IsDigit(str[pos]))
            {
                if (char.IsDigit(str[pos]))
                {
                    integerPart = integerPart * 10 + (str[pos] - '0');
                    pos++;
                }
                else
                {
                    break;
                }
            }

            // 解析小数部分（遇到小数点触发）
            if (pos < length && str[pos] == '.')
            {
                pos++;
                // 最多解析10位小数
                while ((pos < length) && char.IsDigit(str[pos]) && (fractionalDigits < 10))
                {
                    fractionalPart = fractionalPart * 10 + (str[pos] - '0');
                    fractionalDigits++;
                    pos++;
                }
                // 跳过剩余多余的小数位
            }

            // 计算绝对值对应的定点原始值
            long absRaw = integerPart * Scale;

            if (fractionalDigits > 0)
            {
                // 计算10的小数位数次方
                long pow10 = 1;
                for (int i = 0; i < fractionalDigits; i++)
                    pow10 *= 10;

                // 四舍五入计算小数部分对应的定点值
                long fracRaw = (fractionalPart * Scale + pow10 / 2) / pow10;
                absRaw += fracRaw;
            }

            // 6. 范围校验与符号转换（32位有符号补码规则）
            const long MaxPositiveRaw = 0x7FFFFFFF;   // 正数最大值：2^31 - 1
            const long MinNegativeAbsRaw = 0x80000000; // 负数绝对值最大值：2^31
            uint rawValue;

            if (sign == 1)
            {
                // 正数饱和截断
                if (absRaw > MaxPositiveRaw) absRaw = MaxPositiveRaw;
                rawValue = (uint)absRaw;
            }
            else
            {
                // 负数饱和截断
                if (absRaw > MinNegativeAbsRaw)
                    absRaw = MinNegativeAbsRaw;

                // 处理int最小值边界，避免强制转换溢出
                if (absRaw == MinNegativeAbsRaw)
                {
                    rawValue = (uint)MinNegativeAbsRaw;
                }
                else
                {
                    rawValue = (uint)(-(int)absRaw);
                }
            }

            return new FPT(rawValue);
        }

        /// <summary>
        /// 从字符串读取值并初始化到定点数结构
        /// </summary>
        /// <param name="str">指向格式为十进制小数的字符串指针</param>
        /// <param name="length">该字符串长度</param>
        /// <returns>从<paramref name="str"/>读取的定点数值</returns>
        /// <exception cref="ArgumentException">空指针或长度不大于0</exception>
        public static FPT CreateFromString(CPtr<char> str, int length)
        {
            if (str.IsEmpty || length <= 0) throw new ArgumentException();
            return f_createFromStrBuf(str, length);
        }

        /// <summary>
        /// 从指定字符串创建定点数
        /// </summary>
        /// <param name="value">表示一个定点数文本的字符串</param>
        /// <returns>定点数值</returns>
        /// <exception cref="ArgumentException">参数是null或空字符串</exception>
        public static FPT CreateFromString(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException();
            fixed (char* cp = value)
            {
                return f_createFromStrBuf(cp, value.Length);
            }
        }

        /// <summary>
        /// 从字符串创建定点数
        /// </summary>
        /// <param name="value">表示定点数的字符串文本</param>
        /// <param name="index">要从指定索引开始读取文本</param>
        /// <param name="count">要读取的字符数量</param>
        /// <returns>定点数</returns>
        /// <exception cref="ArgumentException">字符串是null或参数超出范围</exception>
        public static FPT CreateFromString(string value, int index, int count)
        {
            if (value is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count > value.Length)) throw new ArgumentOutOfRangeException();
            if (count == 0) return Zero;
            fixed (char* cp = value)
            {
                return f_createFromStrBuf(cp, value.Length);
            }
        }

        /// <summary>
        /// 从字符数组中读取表示定点数的文本并创建定点数
        /// </summary>
        /// <param name="buffer">要读取的字符数组</param>
        /// <returns>创建的定点数</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static FPT CreateFromCharBuffer(char[] buffer)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (buffer.Length == 0) return Zero;
            fixed (char* p = buffer)
            {
                return f_createFromStrBuf(p, buffer.Length);
            }
        }

        /// <summary>
        /// 从字符数组中读取表示定点数的文本并创建定点数
        /// </summary>
        /// <param name="buffer">要读取的字符数组</param>
        /// <param name="index">从指定索引开始读取</param>
        /// <param name="count">要读取的字符数量</param>
        /// <returns>创建的定点数</returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ArgumentException">参数超出范围</exception>
        public static FPT CreateFromCharBuffer(char[] buffer, int index, int count)
        {
            if (buffer is null) throw new ArgumentNullException();
            if (index < 0 || count < 0 || (index + count > buffer.Length)) throw new ArgumentOutOfRangeException();
            if (count == 0) return Zero;
            fixed (char* p = buffer)
            {
                return f_createFromStrBuf(p + index, count);
            }
        }

        #endregion

        #region 数值转换

        /// <summary>
        /// 使用整数值转换为定点数
        /// </summary>
        /// <param name="value">要初始化到定点数的整数值</param>
        /// <returns>表示整数<paramref name="value"/>的定点数</returns>
        public static FPT IntToFPT(int value)
        {
            return new FPT((uint)(value << FractionalBits));
        }

        /// <summary>
        /// 将值等价转换到<see cref="FixedPoint32I24P8"/>
        /// </summary>
        /// <returns></returns>
        public FixedPoint32I24P8 ToFPTI24P8()
        {

            return new FixedPoint32I24P8(
                ((uint)(IntegerRaw << 8)) |
                (((uint)FractionalRaw) >> 2));
        }

        /// <summary>
        /// 将值等价扩展到64位定点数
        /// </summary>
        /// <returns></returns>
        public FixedPoint64I48P16 ToFPT64()
        {
            return new FixedPoint64I48P16(
                ((ulong)(((long)IntegerRaw) << 16)) |
                (((ulong)FractionalRaw) << 8));
        }

        #endregion

        #endregion

        #region 派生

        #region 字符串

#if DEBUG
        /// <summary>
        /// 10^10 / 2^10，用于小数转10位十进制
        /// </summary>
#endif
        private const int FractionalDecMul = 9765625;

        private string f_tostr()
        {
            // 转为有符号long，避免溢出
            long value = ((int)raw);
            if (value == 0) return "0";


            bool isNegative = value < 0;
            long absValue = isNegative ? -value : value;

            // 拆分整数与小数
            long integerPart = absValue >> FractionalBits;
            int fractionalPart = (int)(absValue & (Scale - 1));

            // 整数字符串
            string result = isNegative ? "-" + integerPart.ToString() : integerPart.ToString();

            // 处理小数部分
            if (fractionalPart != 0)
            {
                // 8位十进制小数的整数值
                long fracDec = (long)fractionalPart * FractionalDecMul;
                // 前导零8位固定长度
                string fracStr = fracDec.ToString("D10");

                // 移除末尾无意义零
                int lastNonZeroIndex = fracStr.Length - 1;
                while (lastNonZeroIndex >= 0 && fracStr[lastNonZeroIndex] == '0')
                    lastNonZeroIndex--;

                result += "." + fracStr.Substring(0, lastNonZeroIndex + 1);
            }

            return result;
        }

        private string f_toStringFixed()
        {
            long value = (int)raw;
            bool isNegative = value < 0;
            long absValue = isNegative ? -value : value;

            long integerPart = absValue >> FractionalBits;
            int fractionalPart = (int)(absValue & (Scale - 1));
            var fracDec = ((long)fractionalPart) * FractionalDecMul;

            return $"{(isNegative ? "-" : string.Empty)}{integerPart}.{fracDec:D10}";
        }

        /// <summary>
        /// 返回固定小数长度的字符串
        /// </summary>
        /// <returns></returns>
        public string ToFixedString()
        {
            return f_toStringFixed();
        }

        /// <summary>
        /// 返回以字符串格式的定点数
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return f_tostr();
        }

        #endregion

        #region 运算符

        public static bool operator ==(FPT left, FPT right)
        {
            return left.raw == right.raw;
        }

        public static bool operator !=(FPT left, FPT right)
        {
            return left.raw != right.raw;
        }

        public static bool operator <(FPT left, FPT right)
        {
            return ((int)left.raw) < ((int)right.raw);
        }

        public static bool operator >(FPT left, FPT right)
        {
            return ((int)left.raw) > ((int)right.raw);
        }

        public static bool operator <=(FPT left, FPT right)
        {
            return ((int)left.raw) <= ((int)right.raw);
        }

        public static bool operator >=(FPT left, FPT right)
        {
            return ((int)left.raw) >= ((int)right.raw);
        }

        public static FPT operator +(FPT left, FPT right)
        {
            return Add(left, right);
        }

        public static FPT operator -(FPT left, FPT right)
        {
            return Sub(left, right);
        }

        public static FPT operator *(FPT left, FPT right)
        {
            return Mult(left, right);
        }

        public static FPT operator /(FPT left, FPT right)
        {
            return Div(left, right);
        }

        #endregion

        #region 类型转换

        public static explicit operator FPT(int value)
        {
            return IntToFPT(value);
        }

        public static explicit operator FPT(long value)
        {
            return IntToFPT((int)value);
        }

        public static explicit operator FixedPoint32I24P8(FPT value)
        {
            return value.ToFPTI24P8();
        }

        public static explicit operator FixedPoint64I48P16(FPT value)
        {
            return value.ToFPT64();
        }

        #endregion

        #region 接口重写

        public bool Equals(FPT other)
        {
            return raw == other.raw;
        }

        public int CompareTo(FPT other)
        {
            return ((int)raw).CompareTo((int)other.raw);
        }

        public override bool Equals(object obj)
        {
            if (obj is FPT other) return this == other; return false;
        }

        public override int GetHashCode()
        {
            return (int)raw;
        }

        #endregion

        #endregion

    }

}
