using System;
using System.Collections.Generic;
using System.Collections;
using System.Numerics;
using System.Text;
using System.IO;

using Cheng.Memorys;
using Cheng.Algorithm.HashCodes;

using FPT = Cheng.DataStructure.FixedPoints.FixedPoint64I48P16;
using FPRAW = System.UInt64;

namespace Cheng.DataStructure.FixedPoints
{

    /// <summary>
    /// 64位定点数 - 整数位48bit 小数位16bit
    /// </summary>
    /// <remarks>
    /// <para>
    /// 64位定点数，采用8字节整数最为基本数据，前16bit是小数位，剩余48bit高位是整数位，共同组合成为定点小数<br/>
    /// 位结构如下所示，使用G作为整数bit，X作为小数bit，N作为符号位:<br/>
    /// <code>NGGGGGGG GGGGGGGG GGGGGGGG GGGGGGGG GGGGGGGG GGGGGGGG.XXXXXXXX XXXXXXXX</code>
    /// </para>
    /// <para>定点数的运算结果具有严格的跨平台一致性</para>
    /// </remarks>
    public readonly unsafe struct FixedPoint64I48P16 : IEquatable<FPT>, IComparable<FPT>, IHashCode64
    {

        #region 初始化

        /// <summary>
        /// 使用原始数据初始化定点数
        /// </summary>
        /// <param name="raw"></param>
        public FixedPoint64I48P16(FPRAW raw)
        {
            this.raw = raw;
        }

        #endregion

        #region 参数

        /// <summary>
        /// 64位定点数原始数据
        /// </summary>
        public readonly FPRAW raw;

        #endregion

        #region 功能

        #region 常量

        /// <summary>
        /// 小数位数
        /// </summary>
        public const int FractionalBits = 16;

        /// <summary>
        /// 整数位数（含符号位）
        /// </summary>
        public const int IntegerBits = 48;

#if DEBUG
        /// <summary>
        /// 1.0 对应的原始值
        /// </summary>
#endif
        private const long cp_ONE = 1L << FractionalBits;

#if DEBUG
        /// <summary>
        /// 0.5 用于四舍五入
        /// </summary>
#endif
        private const long cp_HALF = 1L << (FractionalBits - 1);

#if DEBUG
        /// <summary>
        /// 小数部分掩码
        /// </summary>
#endif
        private const long cp_FRAC_MASK = cp_ONE - 1;

#if DEBUG
        /// <summary>
        /// ln(2) 的 Q16 近似值
        /// </summary>
#endif
        private const long cp_LN2 = 45426L;

#if DEBUG
        /// <summary>
        /// log2(e) 的 Q16 近似值
        /// </summary>
#endif
        private const long cp_LOG2_E = 94548L;


        /// <summary>
        /// 表示64位定点数最大值的原始整数
        /// </summary>
        public const FPRAW MaxRaw = ~MinRaw;

        /// <summary>
        /// 表示64位定点数最小值的原始整数
        /// </summary>
        public const FPRAW MinRaw = (1UL << 63);

        /// <summary>
        /// 64位定点数最接近0的绝对值的原始整数
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
        /// <value>返回true表示当前值小于0；返回false表示当前值大于0</value>
        public bool IsNeg
        {
            get
            {
                return ((long)raw) < 0;
            }
        }

        /// <summary>
        /// 当前值是否等于0
        /// </summary>
        public bool IsZero
        {
            get
            {
                return ((long)raw) == 0;
            }
        }

        /// <summary>
        /// 获取小数位原始整数
        /// </summary>
        public long FractionalRaw
        {
            get => (long)(raw & 0b11111111_11111111UL);
        }

        /// <summary>
        /// 获取整数位原始整数
        /// </summary>
        public long IntegerRaw
        {
            get => (((long)raw) >> 16);
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
            return new FPT((FPRAW)(((long)left.raw) + ((long)right.raw)));
        }

        /// <summary>
        /// 计算两数相减之差
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static FPT Sub(FPT left, FPT right)
        {
            return new FPT((FPRAW)(((long)left.raw) - ((long)right.raw)));
        }

        /// <summary>
        /// 乘法运算
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static FPT Mult(FPT left, FPT right)
        {
            return new FPT((FPRAW)Mul((long)left.raw, (long)right.raw));
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
            if (right.raw == 0) throw new DivideByZeroException();

            // 被除数左移 16 位后再除，结果为 Q16 格式
            long a = (long)left.raw;
            long b = (long)right.raw;
            return new FPT((FPRAW)((a << FractionalBits) / b));
        }

        #endregion

        #region 高级运算

        /// <summary>
        /// 获取绝对值
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static FPT Abs(FPT num)
        {
            long value = (long)num.raw;
            if (value < 0) value = -value;
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
            if (((long)x.raw) <= 0) throw new ArgumentException();
            long exponent = Mul((long)y.raw, Ln((long)x.raw));
            return new FPT((FPRAW)((Exp(new FPT((FPRAW)exponent)).raw)));
        }

        /// <summary>
        /// 计算值的平方根
        /// </summary>
        /// <param name="num"></param>
        /// <returns>返回值的平方根</returns>
        /// <exception cref="ArgumentException">值小于0</exception>
        public static FPT Sqrt(FPT num)
        {
            if (num.IsNeg) throw new ArgumentException();
            if (num.IsZero) return num;

            ulong a = num.raw;
            ulong aShifted = a << FractionalBits;

            // 初始值估算：最高位右移一半
            int bit = HighestBit(aShifted);
            ulong x = 1UL << (bit / 2);

            // 牛顿迭代法
            for (int i = 0; i < 10; i++)
            {
                x = (x + aShifted / x) >> 1;
            }

            return new FPT(x);
        }

        /// <summary>
        /// 自然指数
        /// </summary>
        /// <param name="num"></param>
        /// <returns>e的<paramref name="d"/>次幂</returns>
        public static FPT Exp(FPT d)
        {
            // e^x = 2^(x * log2(e))，转换为2的幂次运算
            long y = Mul((long)d.raw, cp_LOG2_E);

            // 拆分为整数部分k和小数部分f，f归一化到[-0.5, 0.5]提升精度
            var k = (int)(y >> FractionalBits);
            long f = y & cp_FRAC_MASK;

            if (f > cp_HALF)
            {
                k += 1;
                f -= cp_ONE;
            }

            // 计算2的整数次幂（直接移位）
            long pow2Int;
            var shift = k + FractionalBits;
            if (shift >= 0)
            {
                pow2Int = shift >= 63 ? long.MaxValue : (1L << shift);
            }
            else
            {
                pow2Int = cp_ONE >> (-k);
            }

            // 定点乘法合并结果
            return new FPT((FPRAW)Mul(pow2Int, Pow2Fraction(f)));
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

        #region 字符串

        private static FPT f_createFromStrBuf(char* p, int length)
        {
            bool isNeg = false;
            int pos = 0;

            // 处理符号位
            if (p[0] == '-')
            {
                isNeg = true;
                pos = 1;
            }
            else if (p[0] == '+')
            {
                pos = 1;
            }

            long integerPart = 0;
            long fractionalPart = 0;
            int fracDigits = 0;
            bool hasDot = false;

            for (int fc = 0; (pos < length) && (fc < 16); pos++, fc++)
            {
                char c = p[pos];

                if (c == '.' || c == ',')
                {
                    if (hasDot) break;
                    hasDot = true;
                    continue;
                }

                if (c < '0' || c > '9') break;

                int digit = c - '0';
                if (!hasDot)
                {
                    integerPart = integerPart * 10 + digit;
                }
                else
                {
                    fractionalPart = fractionalPart * 10 + digit;
                    fracDigits++;
                }
            }

            // 整数部分左移16位
            long raw = integerPart << FractionalBits;

            // 小数部分转换并四舍五入
            if (fracDigits > 0)
            {
                long divisor = 1;
                for (int i = 0; i < fracDigits; i++) divisor *= 10;

                raw += (fractionalPart * cp_ONE + divisor / 2) / divisor;
            }

            if (isNeg) raw = -raw;

            return new FPT((FPRAW)raw);
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
        /// 从字符串创建定点数
        /// </summary>
        /// <param name="value">表示定点数的字符串文本</param>
        /// <returns>定点数</returns>
        /// <exception cref="ArgumentException">字符串是null或空字符串</exception>
        public static FPT CreateFromString(string value)
        {
            if(string.IsNullOrEmpty(value)) throw new ArgumentNullException();
            fixed (char* cp = value)
            {
                return f_createFromStrBuf(cp, value.Length);
            }
        }

        /// <summary>
        /// 从字符串创建定点数
        /// </summary>
        /// <param name="strBuffer">指向表示定点数的字符串文本</param>
        /// <param name="length">字符串的字符数量</param>
        /// <returns>定点数</returns>
        /// <exception cref="ArgumentException">字符串指向null或空字符串</exception>
        public static FPT CreateFromString(CPtr<char> strBuffer, int length)
        {
            if (strBuffer.IsEmpty || length <= 0) throw new ArgumentException();
            return f_createFromStrBuf(strBuffer, length);
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

        private void f_toString(TextWriter sb)
        {
            long value = (long)raw;
            bool isNeg = value < 0;

            if (isNeg)
                value = -value;

            long integerPart = value >> FractionalBits;
            long fracPart = value & cp_FRAC_MASK;

            if (isNeg && value != long.MinValue) sb.Write('-');

            sb.Write(integerPart);

            if (fracPart != 0)
            {
                sb.Write('.');
                long frac = fracPart;

                // 16位二进制小数对应十进制
                for (int i = 0; i < 16; i++)
                {
                    frac *= 10;
                    int digit = (int)(frac >> FractionalBits);
                    sb.Write((char)('0' + digit));
                    frac &= cp_FRAC_MASK;

                    if (frac == 0) break;

                }
            }
        }

        /// <summary>
        /// 返回定点数字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            using(StringWriter swr = new StringWriter())
            {
                f_toString(swr);
                return swr.ToString();
            }
        }

        /// <summary>
        /// 将定点数转换为文本字符串
        /// </summary>
        /// <param name="append">要写入的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public void ToStringAppend(StringBuilder append)
        {
            f_toString(new StringWriter(append));
        }

        /// <summary>
        /// 将定点数转换为文本字符串
        /// </summary>
        /// <param name="writer">要写入的字符串缓冲区</param>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        /// <exception cref="ObjectDisposedException">对象已释放</exception>
        /// <exception cref="IOException">IO错误</exception>
        public void ToStringAppend(TextWriter writer)
        {
            f_toString(writer ?? throw new ArgumentNullException());
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
            return new FPT((ulong)(((long)value) << FractionalBits));
        }

        /// <summary>
        /// 使用整数值转换为定点数
        /// </summary>
        /// <param name="value">要初始化到定点数的整数值</param>
        /// <returns>表示整数<paramref name="value"/>的定点数</returns>
        public static FPT IntToFPT(long value)
        {
            return new FPT((ulong)((value) << FractionalBits));
        }

        /// <summary>
        /// 将值等价截取到<see cref="FixedPoint32I22P10"/>
        /// </summary>
        /// <returns></returns>
        public FixedPoint32I22P10 ToFPTI22P10()
        {

            return new FixedPoint32I22P10(
                ((uint)(((int)IntegerRaw) << 10)) |
                ((((uint)FractionalRaw) >> 6) & 0x3FF));
        }

        /// <summary>
        /// 将值等价截取到<see cref="FixedPoint32I24P8"/>
        /// </summary>
        /// <returns></returns>
        public FixedPoint32I24P8 ToFPTI24P8()
        {

            return new FixedPoint32I24P8(
                ((uint)(((int)IntegerRaw) << 8)) |
                ((((uint)FractionalRaw) >> 8) & 0xFF));
        }

        #endregion

        #region 封装

#if DEBUG
        /// <summary>
        /// 纯整数定点乘法：两个Q16数相乘，返回Q16结果
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
#endif
        private static long Mul(long a, long b)
        {
            bool negative = (a < 0) ^ (b < 0);

            // 取绝对值转为无符号，避免long.MinValue取反溢出
            ulong absA = a >= 0 ? (ulong)a : (ulong)(~a) + 1;
            ulong absB = b >= 0 ? (ulong)b : (ulong)(~b) + 1;

            // 64位无符号乘法得到128位结果
            Mul64(absA, absB, out ulong hi, out ulong lo);

            // 整体右移16位得到Q16结果
            ulong result = (lo >> 16) | (hi << (64 - 16));

            long res = (long)result;
            return negative ? -res : res;
        }

#if DEBUG
        /// <summary>
        /// 64位无符号乘法，输出128位结果的高64位和低64位
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="hi"></param>
        /// <param name="lo"></param>
#endif
        private static void Mul64(ulong a, ulong b, out ulong hi, out ulong lo)
        {
            ulong aHi = a >> 32;
            ulong aLo = a & 0xFFFFFFFF;
            ulong bHi = b >> 32;
            ulong bLo = b & 0xFFFFFFFF;

            ulong p0 = aLo * bLo;
            ulong p1 = aLo * bHi;
            ulong p2 = aHi * bLo;
            //ulong p3 = aHi * bHi;

            ulong mid = p1 + p2;
            ulong carry = mid < p1 ? 1UL << 32 : 0;

            hi = (aHi * bHi) + (mid >> 32) + carry;
            lo = p0 + (mid << 32);

            if (lo < p0) hi += 1;
        }

#if DEBUG
        /// <summary>
        /// 获取无符号数最高有效位位置（0-based）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
#endif
        private static int HighestBit(ulong value)
        {
            if (value == 0) return -1;

            int bit = 0;
            if ((value >> 32) != 0) { bit += 32; value >>= 32; }
            if ((value >> 16) != 0) { bit += 16; value >>= 16; }
            if ((value >> 8) != 0) { bit += 8; value >>= 8; }
            if ((value >> 4) != 0) { bit += 4; value >>= 4; }
            if ((value >> 2) != 0) { bit += 2; value >>= 2; }
            if ((value >> 1) != 0) { bit += 1; }
            return bit;
        }

#if DEBUG
        /// <summary>
        /// 自然对数（纯整数Q16实现）
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
#endif
        private static long Ln(long x)
        {
            if (x <= 0) throw new ArgumentException("Value must be positive");

            // 范围缩减：归一化到 [1, 2) 区间
            int k = 0;
            long m = x;

            while (m >= 2 * cp_ONE)
            {
                m >>= 1;
                k++;
            }
            while (m < cp_ONE)
            {
                m <<= 1;
                k--;
            }

            // t = (m-1)/(m+1)，转换为Q16格式
            long numerator = m - cp_ONE;
            long denominator = m + cp_ONE;
            long t = (numerator << FractionalBits) / denominator;

            // 级数展开：ln(m) = 2*(t + t^3/3 + t^5/5 + t^7/7 + ...)
            long sum = t;
            long tSq = Mul(t, t);
            long term = t;
            int n = 1;

            for (int i = 1; i < 6; i++)
            {
                term = Mul(term, tSq);
                n += 2;
                sum += term / n;
            }

            long lnM = sum << 1;

            // ln(x) = ln(m) + k * ln2
            return lnM + k * cp_LN2;
        }

#if DEBUG
        /// <summary>
        /// 计算2的小数次幂（f ∈ [-0.5, 0.5]）
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
#endif
        private static long Pow2Fraction(long f)
        {
            // 泰勒展开：2^f = 1 + f*ln2 + (f*ln2)^2/2! + (f*ln2)^3/3! + ...
            long sum = cp_ONE;
            long term = cp_ONE;

            for (int i = 1; i < 10; i++)
            {
                term = Mul(term, f);
                term = Mul(term, cp_LN2);
                term /= i;

                sum += term;

                if (term == 0)
                    break;
            }

            return sum;
        }

        #endregion

        #endregion

        #region 派生

        #region 运算符重载

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
            return ((long)left.raw) < ((long)right.raw);
        }

        public static bool operator >(FPT left, FPT right)
        {
            return ((long)left.raw) > ((long)right.raw);
        }

        public static bool operator <=(FPT left, FPT right)
        {
            return ((long)left.raw) <= ((long)right.raw);
        }

        public static bool operator >=(FPT left, FPT right)
        {
            return ((long)left.raw) >= ((long)right.raw);
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

        public static implicit operator FPT(int value)
        {
            return IntToFPT(value);
        }

        public static explicit operator FPT(long value)
        {
            return IntToFPT(value);
        }

        public static explicit operator FixedPoint32I24P8(FPT value)
        {
            return value.ToFPTI24P8();
        }

        public static explicit operator FixedPoint32I22P10(FPT value)
        {
            return value.ToFPTI22P10();
        }

        #endregion

        #region 接口

        public bool Equals(FPT other)
        {
            return raw == other.raw;
        }

        public int CompareTo(FPT other)
        {
            return ((long)raw).CompareTo((long)other.raw);
        }

        public override bool Equals(object obj)
        {
            if (obj is FPT other) return raw == other.raw; return false;
        }

        public override int GetHashCode()
        {
            return raw.GetHashCode();
        }

        public long GetHashCode64()
        {
            return (long)raw;
        }

        #endregion

        #endregion

    }

}
