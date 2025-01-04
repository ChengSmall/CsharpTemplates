using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

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
        public static double Sqrt2
        {
            get
            {
                ulong n = 0x3FF6A09E667F3BCD;
                return *((double*)&n);
            }
        }

        /// <summary>
        /// 3的平方根
        /// </summary>
        public static double Sqrt3
        {
            get
            {
                ulong n = 0x3FFBB67AE8584CAA;
                return *((double*)&n);
            }
        }

        /// <summary>
        /// 真空中光线一秒钟的路程，单位米
        /// </summary>
        public const int LightSpeed = 299792458;

        #endregion

        #region 大小计算

        /// <summary>
        /// 将值限制在指定范围内
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static float Clamp(float value, float min, float max)
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
        public static double Clamp(double value, double min, double max)
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
        public static int Clamp(int value, int min, int max)
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
        public static long Clamp(long value, long min, long max)
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
        public static decimal Clamp(decimal value, decimal min, decimal max)
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
        public static uint Clamp(uint value, uint min, uint max)
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
        public static ulong Clamp(ulong value, ulong min, ulong max)
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
        public static T Clamp<T>(T value, T min, T max, IComparer<T> comparer)
        {
            if (comparer is null) comparer = Comparer<T>.Default;

            if (comparer.Compare(value, max) > 0) return max;
            if (comparer.Compare(value, min) < 0) return min;
            return value;
        }
        /// <summary>
        /// 将值限制在指定范围内，使用默认比较器比较大小
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>当值小于最小值时返回最小值，大于最大值时返回最大值，否则返回值本身</returns>
        public static T Clamp<T>(T value, T min, T max)
        {
            var comparer = Comparer<T>.Default;
            if (comparer.Compare(value, max) > 0) return max;
            if (comparer.Compare(value, min) < 0) return min;
            return value;
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
        /// 比较三个值的大小，使用默认比较强
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



        #endregion

        #region 最大公约数

        /// <summary>
        /// 求两值最大公约数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>最大公约数</returns>
        /// <exception cref="ArgumentException">有值为0</exception>
        public static long GreatcommonDivisor(long x, long y)
        {
            if (x == 0 || y == 0) throw new ArgumentException();

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
        /// <returns>最大公约数</returns>
        /// <exception cref="ArgumentException">有值为0</exception>
        public static long GreatcommonDivisor(int x, int y)
        {
            if (x == 0 || y == 0) throw new ArgumentException();

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
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        public static long GreatcommonDivisor(this IList<long> array)
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
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        public static int GreatcommonDivisor(this IList<int> array)
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
        public static long GreatcommonDivisor<T>(this IList<T> list, Func<T, long> toNum)
        {
            if (list is null) throw new ArgumentNullException();

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

    }
}
