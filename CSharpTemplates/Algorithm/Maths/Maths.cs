using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Numerics;

namespace Cheng.Algorithm
{

    /// <summary>
    /// 数学计算扩展
    /// </summary>
    public unsafe static partial class Maths
    {

        #region 常量

        /// <summary>
        /// 2的平方根
        /// </summary>
        public const double Sqrt2 = 1.4142135623730951;

        /// <summary>
        /// 2的平方根
        /// </summary>
        public const float FSqrt2 = 1.41421354f;

        /// <summary>
        /// 3的平方根
        /// </summary>
        public const double Sqrt3 = 1.7320508075688772;

        /// <summary>
        /// 3的平方根
        /// </summary>
        public const float FSqrt3 = 1.73205078f;

        /// <summary>
        /// 0.5的平方根
        /// </summary>
        public const double Sqrt0p5 = 0.70710678118654757;

        /// <summary>
        /// 0.5的平方根
        /// </summary>
        public const float FSqrt0p5 = 0.707106769f;

        /// <summary>
        /// 真空中光线一秒钟的路程，单位米
        /// </summary>
        public const int LightSpeed = 299792458;

        #endregion

        #region 大小计算

        #region Clamp

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static float Clamp(this float value, float min, float max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static double Clamp(this double value, double min, double max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static int Clamp(this int value, int min, int max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static long Clamp(this long value, long min, long max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static decimal Clamp(this decimal value, decimal min, decimal max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static uint Clamp(this uint value, uint min, uint max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static ulong Clamp(this ulong value, ulong min, ulong max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static char Clamp(this char value, char min, char max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

        /// <summary>
        /// 将值限制在指定范围内，使用自定义比较器比较大小
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="comparer">定义值比较器，若为null则使用默认比较器</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static T Clamp<T>(this T value, T min, T max, IComparer<T> comparer)
        {
            if (comparer is null) comparer = Comparer<T>.Default;
            var re = comparer.Compare(value, max);
            if (re > 0) return max;
            if (re < 0) return min;
            return value;
        }

        /// <summary>
        /// 将值限制在指定范围内，使用默认比较器比较大小
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static T Clamp<T>(this T value, T min, T max)
        {
            return Clamp(value, min, max, Comparer<T>.Default);
        }

        /// <summary>
        /// 限制值的最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="max">最大值</param>
        /// <returns>如果值大于<paramref name="max"/>返回<paramref name="max"/>，否则返回值本身</returns>
        public static int ClampMax(this int value, int max)
        {
            return value > max ? max : value;
        }

        /// <summary>
        /// 限制值的最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="max">最大值</param>
        /// <returns>如果值大于<paramref name="max"/>返回<paramref name="max"/>，否则返回值本身</returns>
        public static uint ClampMax(this uint value, uint max)
        {
            return value > max ? max : value;
        }

        /// <summary>
        /// 限制值的最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="max">最大值</param>
        /// <returns>如果值大于<paramref name="max"/>返回<paramref name="max"/>，否则返回值本身</returns>
        public static long ClampMax(this long value, long max)
        {
            return value > max ? max : value;
        }

        /// <summary>
        /// 限制值的最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="max">最大值</param>
        /// <returns>如果值大于<paramref name="max"/>返回<paramref name="max"/>，否则返回值本身</returns>
        public static ulong ClampMax(this ulong value, ulong max)
        {
            return value > max ? max : value;
        }

        /// <summary>
        /// 限制值的最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="max">最大值</param>
        /// <returns>如果值大于<paramref name="max"/>返回<paramref name="max"/>，否则返回值本身</returns>
        public static float ClampMax(this float value, float max)
        {
            return value > max ? max : value;
        }

        /// <summary>
        /// 限制值的最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="max">最大值</param>
        /// <returns>如果值大于<paramref name="max"/>返回<paramref name="max"/>，否则返回值本身</returns>
        public static double ClampMax(this double value, double max)
        {
            return value > max ? max : value;
        }

        /// <summary>
        /// 限制值的最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="max">最大值</param>
        /// <returns>如果值大于<paramref name="max"/>返回<paramref name="max"/>，否则返回值本身</returns>
        public static decimal ClampMax(this decimal value, decimal max)
        {
            return value > max ? max : value;
        }

        /// <summary>
        /// 限制值的最大值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="max">最大值</param>
        /// <returns>如果值大于<paramref name="max"/>返回<paramref name="max"/>，否则返回值本身</returns>
        public static char ClampMax(this char value, char max)
        {
            return value > max ? max : value;
        }

        /// <summary>
        /// 限制值的最小值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <returns>如果值小于<paramref name="min"/>返回<paramref name="min"/>，否则返回值本身</returns>
        public static int ClampMin(this int value, int min)
        {
            return value < min ? min : value;
        }

        /// <summary>
        /// 限制值的最小值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <returns>如果值小于<paramref name="min"/>返回<paramref name="min"/>，否则返回值本身</returns>
        public static uint ClampMin(this uint value, uint min)
        {
            return value < min ? min : value;
        }

        /// <summary>
        /// 限制值的最小值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <returns>如果值小于<paramref name="min"/>返回<paramref name="min"/>，否则返回值本身</returns>
        public static long ClampMin(this long value, long min)
        {
            return value < min ? min : value;
        }

        /// <summary>
        /// 限制值的最小值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <returns>如果值小于<paramref name="min"/>返回<paramref name="min"/>，否则返回值本身</returns>
        public static ulong ClampMin(this ulong value, ulong min)
        {
            return value < min ? min : value;
        }

        /// <summary>
        /// 限制值的最小值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <returns>如果值小于<paramref name="min"/>返回<paramref name="min"/>，否则返回值本身</returns>
        public static float ClampMin(this float value, float min)
        {
            return value < min ? min : value;
        }

        /// <summary>
        /// 限制值的最小值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <returns>如果值小于<paramref name="min"/>返回<paramref name="min"/>，否则返回值本身</returns>
        public static double ClampMin(this double value, double min)
        {
            return value < min ? min : value;
        }

        /// <summary>
        /// 限制值的最小值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <returns>如果值小于<paramref name="min"/>返回<paramref name="min"/>，否则返回值本身</returns>
        public static decimal ClampMin(this decimal value, decimal min)
        {
            return value < min ? min : value;
        }

        /// <summary>
        /// 限制值的最小值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <returns>如果值小于<paramref name="min"/>返回<paramref name="min"/>，否则返回值本身</returns>
        public static char ClampMin(this char value, char min)
        {
            return value < min ? min : value;
        }

        #endregion

        #region max

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static sbyte Max(sbyte a, sbyte b, sbyte c)
        {
            if (a > b) return a > c ? a : c;
            return b > c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static byte Max(byte a, byte b, byte c)
        {
            if (a > b) return a > c ? a : c;
            return b > c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static ushort Max(ushort a, ushort b, ushort c)
        {
            if (a > b) return a > c ? a : c;
            return b > c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static short Max(short a, short b, short c)
        {
            if (a > b) return a > c ? a : c;
            return b > c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static int Max(int a, int b, int c)
        {
            if(a > b) return a > c ? a : c;
            return b > c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static uint Max(uint a, uint b, uint c)
        {
            if (a > b) return a > c ? a : c;
            return b > c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static long Max(long a, long b, long c)
        {
            if (a > b) return a > c ? a : c;
            return b > c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static ulong Max(ulong a, ulong b, ulong c)
        {
            if (a > b) return a > c ? a : c;
            return b > c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static float Max(float a, float b, float c)
        {
            if (a > b) return a > c ? a : c;
            return b > c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static double Max(double a, double b, double c)
        {
            if (a > b) return a > c ? a : c;
            return b > c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static decimal Max(decimal a, decimal b, decimal c)
        {
            if (a > b) return a > c ? a : c;
            return b > c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="comparer">自定义比较器</param>
        /// <returns>返回其中最大的值</returns>
        public static T Max<T>(T a, T b, T c, IComparer<T> comparer)
        {
            if (comparer is null) comparer = Comparer<T>.Default;
            if (comparer.Compare(a, b) > 0) return (comparer.Compare(a, c) > 0) ? a : c;
            return (comparer.Compare(b, c) > 0) ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小，使用默认实现的<see cref="IComparer"/>比较器比较大小
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最大的值</returns>
        public static T Max<T>(T a, T b, T c)
        {
            IComparer<T> comparer = Comparer<T>.Default;
            if (comparer.Compare(a, b) > 0) return (comparer.Compare(a, c) > 0) ? a : c;
            return (comparer.Compare(b, c) > 0) ? b : c;
        }

        /// <summary>
        /// 选取集合中最大的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">数组</param>
        /// <returns>数组当中最小的值</returns>
        /// <exception cref="ArgumentNullException">数组是null</exception>
        /// <exception cref="ArgumentException">数组元素为0</exception>
        public static T Max<T>(this IEnumerable<T> collection)
        {
            return Max(collection, null);
        }

        /// <summary>
        /// 选取集合中最大的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">数组</param>
        /// <param name="comparer">比较器，null表示使用默认比较器</param>
        /// <returns>数组当中最小的值</returns>
        /// <exception cref="ArgumentNullException">数组是null</exception>
        /// <exception cref="ArgumentException">数组元素为0</exception>
        public static T Max<T>(this IEnumerable<T> collection, IComparer<T> comparer)
        {
            if (collection is null) throw new ArgumentNullException();
            if (comparer is null) comparer = Comparer<T>.Default;
            T temp = default;
            using (var enumator = collection.GetEnumerator())
            {

                if (!enumator.MoveNext())
                {
                    throw new ArgumentException();
                }

                temp = enumator.Current;

                while (enumator.MoveNext())
                {

                    T current = enumator.Current;

                    int cv = comparer.Compare(temp, current);
                    if (cv < 0)
                    {
                        //左小
                        temp = current;
                    }

                }

            }

            return temp;
        }

        #endregion

        #region min

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static int Min(int a, int b, int c)
        {
            if (a < b) return a < c ? a : c;
            return b < c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static uint Min(uint a, uint b, uint c)
        {
            if (a < b) return a < c ? a : c;
            return b < c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static long Min(long a, long b, long c)
        {
            if (a < b) return a < c ? a : c;
            return b < c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static ulong Min(ulong a, ulong b, ulong c)
        {
            if (a < b) return a < c ? a : c;
            return b < c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static float Min(float a, float b, float c)
        {
            if (a < b) return a < c ? a : c;
            return b < c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static double Min(double a, double b, double c)
        {
            if (a < b) return a < c ? a : c;
            return b < c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static decimal Min(decimal a, decimal b, decimal c)
        {
            if (a < b) return a < c ? a : c;
            return b < c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static short Min(short a, short b, short c)
        {
            if (a < b) return a < c ? a : c;
            return b < c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static ushort Min(ushort a, ushort b, ushort c)
        {
            if (a < b) return a < c ? a : c;
            return b < c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static byte Min(byte a, byte b, byte c)
        {
            if (a < b) return a < c ? a : c;
            return b < c ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static sbyte Min(sbyte a, sbyte b, sbyte c)
        {
            if (a < b) return a < c ? a : c;
            return b < c ? b : c;
        }


        /// <summary>
        /// 比较三个值的大小
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="comparer">自定义比较器，若为null则使用默认比较器</param>
        /// <returns>返回其中最小的值</returns>
        public static T Min<T>(T a, T b, T c, IComparer<T> comparer)
        {
            if (comparer is null) comparer = Comparer<T>.Default;
            
            if (comparer.Compare(a, b) < 0) return (comparer.Compare(a, c) < 0) ? a : c;
            return (comparer.Compare(b, c) < 0) ? b : c;
        }

        /// <summary>
        /// 比较三个值的大小，使用默认比较器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>返回其中最小的值</returns>
        public static T Min<T>(T a, T b, T c)
        {
            var comparer = Comparer<T>.Default;
            if (comparer.Compare(a, b) < 0) return (comparer.Compare(a, c) < 0) ? a : c;
            return (comparer.Compare(b, c) < 0) ? b : c;
        }

        /// <summary>
        /// 选取集合中最小的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">数组</param>
        /// <returns>数组当中最小的值</returns>
        /// <exception cref="ArgumentNullException">数组是null</exception>
        /// <exception cref="ArgumentException">数组元素为0</exception>
        public static T Min<T>(this IEnumerable<T> collection)
        {
            return Min(collection, null);
        }

        /// <summary>
        /// 选取集合中最小的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">数组</param>
        /// <param name="comparer">比较器，null表示使用默认比较器</param>
        /// <returns>数组当中最小的值</returns>
        /// <exception cref="ArgumentNullException">数组是null</exception>
        /// <exception cref="ArgumentException">数组元素为0</exception>
        public static T Min<T>(this IEnumerable<T> collection, IComparer<T> comparer)
        {
            if (collection is null) throw new ArgumentNullException();
            if (comparer is null) comparer = Comparer<T>.Default;
            T temp = default;
            using (var enumator = collection.GetEnumerator())
            {

                if (!enumator.MoveNext())
                {
                    throw new ArgumentException();
                }

                temp = enumator.Current;

                while (enumator.MoveNext())
                {

                    T current = enumator.Current;

                    int cv = comparer.Compare(temp, current);
                    if (cv > 0)
                    {
                        //temp大，将较小值写入temp
                        temp = current;
                    }

                }

            }

            return temp;
        }

        #endregion

        #endregion

        #region 最大公约数

        /// <summary>
        /// 求两值最大公约数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns><paramref name="x"/>和<paramref name="y"/>的最大公约数；当有值等于0时返回值为0</returns>
        public static ulong GreatCommonDivisor(ulong x, ulong y)
        {
            if (x == 0 || y == 0) return 0;

            Loop:
            if (x == y) return x;

            if (x < y) y -= x;
            else x -= y;
            goto Loop;
        }

        /// <summary>
        /// 求两值最大公约数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns><paramref name="x"/>和<paramref name="y"/>的最大公约数；当有值等于0时返回值为0</returns>
        /// <exception cref="ArgumentOutOfRangeException">有值小于0</exception>
        public static long GreatCommonDivisor(long x, long y)
        {
            if (x < 0 || y < 0) throw new ArgumentOutOfRangeException();
            return (long)GreatCommonDivisor((ulong)x, (ulong)y);
        }

        /// <summary>
        /// 求两值最大公约数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns><paramref name="x"/>和<paramref name="y"/>的最大公约数；当有值等于0时返回值为0</returns>
        /// <exception cref="ArgumentOutOfRangeException">有值小于0</exception>
        public static int GreatCommonDivisor(int x, int y)
        {
            if (x < 0 || y < 0) throw new ArgumentOutOfRangeException();
            if (x == 0 || y == 0) return 0;
            Loop:
            if (x == y) return x;

            if (x < y) y -= x;
            else x -= y;
            goto Loop;
        }

        /// <summary>
        /// 求最大公约数
        /// </summary>
        /// <param name="array">值集合</param>
        /// <returns>最大公约数</returns>
        /// <exception cref="ArgumentException">数组元素为0</exception>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        public static long GreatCommonDivisor(this IList<long> array)
        {
            if (array is null) throw new ArgumentNullException();

            int length = array.Count;

            if (length == 0) throw new ArgumentException();

            long temp;
            int i;

            //获取最小值
            temp = array[0];
            for (i = 1; i < length; i++)
            {
                if (array[i] < temp) temp = array[i];
            }

            Loop:

            for (i = 0; i < length; i++)
            {

                if(array[i] % temp != 0)
                {
                    //不是temp
                    temp--;
                    goto Loop;
                }

            }

            //是temp

            return temp;

        }

        /// <summary>
        /// 求最大公约数
        /// </summary>
        /// <param name="array">值集合</param>
        /// <returns>最大公约数</returns>
        /// <exception cref="ArgumentException">数组元素为0</exception>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        public static int GreatCommonDivisor(this IList<int> array)
        {
            if (array is null) throw new ArgumentNullException();

            int length = array.Count;

            if (length == 0) throw new ArgumentException();

            int temp;
            int i;

            //获取最小值
            temp = array[0];
            for (i = 1; i < length; i++)
            {
                if (array[i] < temp) temp = array[i];
            }

            Loop:

            for (i = 0; i < length; i++)
            {

                if (array[i] % temp != 0)
                {
                    //不是temp
                    temp--;
                    goto Loop;
                }

            }

            //是temp

            return temp;

        }

        /// <summary>
        /// 求最大公约数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">值集合</param>
        /// <param name="toNum">用于将对象转化为数值的委托</param>
        /// <returns>最大公约数</returns>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        public static long GreatCommonDivisor<T>(this IList<T> list, Func<T, long> toNum)
        {
            if (list is null || toNum is null) throw new ArgumentNullException();

            int length = list.Count;

            if (length == 0) throw new ArgumentException();

            long temp;
            int i;

            //获取最小值
            temp = toNum.Invoke(list[0]);
            for (i = 1; i < length; i++)
            {
                if (toNum.Invoke(list[i]) < temp) temp = toNum.Invoke(list[i]);
            }

            Loop:

            for (i = 0; i < length; i++)
            {

                if (toNum.Invoke(list[i]) % temp != 0)
                {
                    //不是temp
                    temp--;
                    goto Loop;
                }

            }

            //是temp

            return temp;
        }

        /// <summary>
        /// 求最大公约数
        /// </summary>
        /// <param name="array">值集合</param>
        /// <returns>最大公约数</returns>
        /// <exception cref="ArgumentException">数组元素为0</exception>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        public static long GreatCommonDivisor(this IReadOnlyList<long> array)
        {
            if (array is null) throw new ArgumentNullException();

            int length = array.Count;

            if (length == 0) throw new ArgumentException();

            long temp;
            int i;

            //获取最小值
            temp = array[0];
            for (i = 1; i < length; i++)
            {
                if (array[i] < temp) temp = array[i];
            }

            Loop:

            for (i = 0; i < length; i++)
            {

                if (array[i] % temp != 0)
                {
                    //不是temp
                    temp--;
                    goto Loop;
                }

            }

            //是temp

            return temp;

        }

        /// <summary>
        /// 求最大公约数
        /// </summary>
        /// <param name="array">值集合</param>
        /// <returns>最大公约数</returns>
        /// <exception cref="ArgumentException">数组元素为0</exception>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        public static int GreatCommonDivisor(this IReadOnlyList<int> array)
        {
            if (array is null) throw new ArgumentNullException();

            int length = array.Count;

            if (length == 0) throw new ArgumentException();

            int temp;
            int i;

            //获取最小值
            temp = array[0];
            for (i = 1; i < length; i++)
            {
                if (array[i] < temp) temp = array[i];
            }

            Loop:

            for (i = 0; i < length; i++)
            {

                if (array[i] % temp != 0)
                {
                    //不是temp
                    temp--;
                    goto Loop;
                }

            }

            //是temp

            return temp;

        }

        /// <summary>
        /// 求最大公约数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">值集合</param>
        /// <param name="toNum">用于将对象转化为数值的委托</param>
        /// <returns>最大公约数</returns>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static long GreatCommonDivisor<T>(this IReadOnlyList<T> list, Func<T, long> toNum)
        {
            if (list is null || toNum is null) throw new ArgumentNullException();

            int length = list.Count;

            if (length == 0) throw new ArgumentException();

            long temp;
            int i;

            //获取最小值
            temp = toNum.Invoke(list[0]);
            for (i = 1; i < length; i++)
            {
                if (toNum.Invoke(list[i]) < temp) temp = toNum.Invoke(list[i]);
            }

            Loop:

            for (i = 0; i < length; i++)
            {

                if (toNum.Invoke(list[i]) % temp != 0)
                {
                    //不是temp
                    temp--;
                    goto Loop;
                }

            }

            //是temp

            return temp;
        }

        #endregion

        #region 最小公倍数

        /// <summary>
        /// 求两值的最小公倍数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns><paramref name="a"/>和<paramref name="b"/>的最小公倍数；如果其中一个等于0，返回值是0</returns>
        public static ulong LeastCommonMultiple(ulong a, ulong b)
        {
            // 处理边界 其中一个数为0，则最小公倍数为0
            // 根据定义，LCM(n, 0) = 0
            if (a == 0 || b == 0)
            {
                return 0;
            }

            // LCM(a, b) = (a * b) / GCD(a, b)
            ulong gcd;
            if (a == 1 || b == 1)
            {
                gcd = 1;
            }
            else
            {
                gcd = GreatCommonDivisor(a, b);
            }
            return (a / gcd) * b;
        }

        /// <summary>
        /// 求两值的最小公倍数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns><paramref name="a"/>和<paramref name="b"/>的最小公倍数；如果其中一个等于0，返回值是0</returns>
        /// <exception cref="ArgumentOutOfRangeException">存在小于0的值</exception>
        public static long LeastCommonMultiple(long a, long b)
        {
            if (a < 0 || b < 0) throw new ArgumentOutOfRangeException();
            return (long)LeastCommonMultiple((ulong)a, (ulong)b);
        }

        /// <summary>
        /// 求两值的最小公倍数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns><paramref name="a"/>和<paramref name="b"/>的最小公倍数；如果其中一个等于0，返回值是0</returns>
        /// <exception cref="ArgumentOutOfRangeException">存在小于0的值</exception>
        public static long LeastCommonMultiple(int a, int b)
        {
            // 处理边界 其中一个数为0，则最小公倍数为0
            // 根据定义，LCM(n, 0) = 0
            if (a == 0 || b == 0)
            {
                return 0;
            }

            // LCM(a, b) = (a * b) / GCD(a, b)
            int gcd;
            if (a == 1 || b == 1)
            {
                gcd = 1;
            }
            else
            {
                gcd = (int)GreatCommonDivisor((ulong)a, (ulong)b);
            }
            return (((a) / gcd) * (b));
        }

        /// <summary>
        /// 求两值的最小公倍数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns><paramref name="a"/>和<paramref name="b"/>的最小公倍数；如果其中一个等于0，返回值是0</returns>
        /// <exception cref="ArgumentOutOfRangeException">存在小于0的值</exception>
        public static BigInteger LeastCommonMultiple(BigInteger a, BigInteger b)
        {
            if (a.IsZero || b.IsZero)
            {
                return BigInteger.Zero;
            }
            BigInteger gcd;
            if (a.IsOne || b.IsOne)
            {
                gcd = BigInteger.One;
            }
            else
            {
                gcd = BigInteger.GreatestCommonDivisor(a, b);
            }
            return (a / gcd) * b;
        }

        #endregion

        #region 质数

        /// <summary>
        /// 判断是否为质数
        /// </summary>
        /// <param name="value">要判断的值</param>
        /// <returns>如果<paramref name="value"/>是质数返回true，否则返回false；<paramref name="value"/>小于2也返回false</returns>
        public static bool IsPrimeNumber(this uint value)
        {
            if (value <= 1) return false;
            for (uint i = 2; i < value; i++)
            {
                if ((value % i) == 0) return false;
            }
            return true;
        }

        /// <summary>
        /// 判断是否为质数
        /// </summary>
        /// <param name="value">要判断的值</param>
        /// <returns>如果<paramref name="value"/>是质数返回true，否则返回false；<paramref name="value"/>小于2也返回false</returns>
        /// <exception cref="ArgumentOutOfRangeException">值小于0</exception>
        public static bool IsPrimeNumber(this int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException();
            return IsPrimeNumber((uint)value);
        }

        /// <summary>
        /// 判断是否为质数
        /// </summary>
        /// <param name="value">要判断的值</param>
        /// <returns>
        /// 如果<paramref name="value"/>是质数返回true，否则返回false；<paramref name="value"/>小于2也返回false
        /// </returns>
        public static bool IsPrimeNumber(this ulong value)
        {
            if (value <= 1) return false;
            for (ulong i = 2; i < value; i++)
            {
                if ((value % i) == 0) return false;
            }
            return true;
        }

        /// <summary>
        /// 判断是否为质数
        /// </summary>
        /// <param name="value">要判断的值</param>
        /// <returns>如果<paramref name="value"/>是质数返回true，否则返回false；<paramref name="value"/>小于2也返回false</returns>
        /// <exception cref="ArgumentOutOfRangeException">值小于0</exception>
        public static bool IsPrimeNumber(this long value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException();
            return IsPrimeNumber((ulong)value);
        }

        /// <summary>
        /// 质数枚举器
        /// </summary>
        /// <returns>
        /// <para>一个质数枚举器，从2开始枚举所有可能的质数</para>
        /// <para>每当枚举器返回的值是一个0时，表示此次枚举没有找到质数，此时0表示一个占位符；返回值不等于0时，表示此次找到的质数</para>
        /// <para>注意，因为该枚举器不会限制数量，所以请不要使用foreach遍历此枚举器，这会遍历64位整数的所有值从而造成永久的线程堵塞</para>
        /// </returns>
        public static IEnumerable<ulong> PrimeNumbers()
        {
            return PrimeNumbers(ulong.MaxValue);
        }

        /// <summary>
        /// 质数枚举器
        /// </summary>
        /// <param name="count">要枚举的质数数量</param>
        /// <returns>
        /// <para>一个质数枚举器，从2开始枚举所有可能的质数</para>
        /// <para>每当枚举器返回的值是一个0时，表示此次枚举没有找到质数，此时0表示一个占位符；返回值不等于0时，表示此次找到的质数</para>
        /// </returns>
        public static IEnumerable<ulong> PrimeNumbers(ulong count)
        {
            ulong c = 0;
            yield return 2;
            yield return 3;
            for (ulong value = 5; value < ulong.MaxValue && (c < count); value++)
            {
                for (ulong i = 2; i < value; i++)
                {
                    if ((value % i) == 0)
                    {
                        //不是质数
                        goto NotPr;
                    }
                }
                yield return value;
                c++;
                continue;
                NotPr:
                yield return 0;
            }
        }

        #endregion

    }

}
