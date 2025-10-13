using Cheng.Algorithm.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Algorithm.Randoms.Extends
{

    /// <summary>
    /// 随机相关扩展方法
    /// </summary>
    public static class RandomExtends
    {

        #region 洗牌

        /// <summary>
        /// 使用随机生成器将给定的集合打乱顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random">随机器</param>
        /// <param name="list">要打乱的集合</param>
        /// <param name="index">要打乱的范围起始索引</param>
        /// <param name="count">要打乱的元素数</param>
        /// <param name="Chaosfrequency">打乱频率，最小为1</param>
        /// <exception cref="ArgumentOutOfRangeException">参数不正确</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void RandomDisrupt<T>(this BaseRandom random, IList<T> list, int index, int count, int Chaosfrequency)
        {
            if (random is null || list is null) throw new ArgumentNullException();

            if (Chaosfrequency <= 0 || index < 0 || count < 0 || (index + count > list.Count)) throw new ArgumentOutOfRangeException();

            if (count == 0) return;
            int length = count + index;
            int end = length - 1;
            int i;

            for (int k = 0; k < Chaosfrequency; k++)
            {
                for (i = index; i < end; i++)
                {
                    list.f_Swap(i, random.Next(i + 1, length));
                }
            }

        }

        /// <summary>
        /// 使用随机生成器将给定的集合打乱顺序
        /// </summary>
        /// <param name="random">随机器</param>
        /// <param name="list">要打乱的集合</param>
        /// <param name="index">要打乱的范围起始索引</param>
        /// <param name="count">要打乱的元素数</param>
        /// <param name="Chaosfrequency">打乱频率，最小为1</param>
        /// <exception cref="ArgumentException">参数不正确</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void RandomDisrupt(this BaseRandom random, global::System.Collections.IList list, int index, int count, int Chaosfrequency)
        {
            if (random is null || list is null) throw new ArgumentNullException();

            if (Chaosfrequency <= 0 || index < 0 || count < 0 || (index + count > list.Count)) throw new ArgumentOutOfRangeException();

            if (count == 0) return;

            int length = count + index;
            int end = length - 1;
            int i, k, j;

            for (k = 0; k < Chaosfrequency; k++)
            {

                for (i = index; i < end; i++)
                {
                    j = random.Next(i + 1, length);

                    list.f_Swap(i, j);
                }
            }

        }

        /// <summary>
        /// 使用随机生成器将给定的集合打乱顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random">随机器</param>
        /// <param name="list">要打乱的集合</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void RandomDisrupt<T>(this BaseRandom random, IList<T> list)
        {
            if (list is null) throw new ArgumentNullException();
            RandomDisrupt(random, list, 0, list.Count, 2);
        }

        /// <summary>
        /// 使用随机生成器将给定的集合打乱顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="random">随机器</param>
        /// <param name="list">要打乱的集合</param>
        /// <param name="index">要打乱的范围起始索引</param>
        /// <param name="count">要打乱的元素数</param>
        /// <exception cref="ArgumentOutOfRangeException">参数不正确</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void RandomDisrupt<T>(this BaseRandom random, IList<T> list, int index, int count)
        {
            RandomDisrupt(random, list, index, count, 2);
        }

        /// <summary>
        /// 使用随机生成器将给定的集合打乱顺序
        /// </summary>
        /// <param name="random">随机器</param>
        /// <param name="list">要打乱的集合</param>
        /// <param name="index">要打乱的范围起始索引</param>
        /// <param name="count">要打乱的元素数</param>
        /// <exception cref="ArgumentOutOfRangeException">参数不正确</exception>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void RandomDisrupt(this BaseRandom random, global::System.Collections.IList list, int index, int count)
        {
            RandomDisrupt(random, list, index, count, 2);
        }

        /// <summary>
        /// 使用随机生成器将给定的集合打乱顺序
        /// </summary>
        /// <param name="random">随机器</param>
        /// <param name="list">要打乱的集合</param>
        /// <exception cref="ArgumentNullException">参数为null</exception>
        public static void RandomDisrupt(this BaseRandom random, global::System.Collections.IList list)
        {
            if (list is null) throw new ArgumentNullException();
            RandomDisrupt(random, list, 0, list.Count, 2);
        }

        #endregion

        #region 唯一概率

        /// <summary>
        /// 按照概率值挑选有序集合内的归一化值
        /// </summary>
        /// <param name="list">
        /// <para>存放归一化数值的有序集合</para>
        /// <para>该集合内的所有值，全部是范围在[0,1)的浮点值，并且该集合元素的顺序是从小到大排列</para>
        /// <para>如果集合参数不符合要求则不保证返回值符合要求</para>
        /// </param>
        /// <param name="probability">
        /// <para>一个范围区间在[0,1)的浮点值</para>
        /// <para>如果参数大于或等于1则返回值永远是<see cref="ICollection{T}.Count"/>，参数小于0时函数返回-1</para>
        /// </param>
        /// <returns>
        /// <para>集合内匹配顺序范围的元素索引</para>
        /// <para>
        /// 按顺序遍历集合<paramref name="list"/>，判断<paramref name="probability"/>是否处于从0开始到第一个元素的值所在范围并返回0，如果不匹配则继续判断第一个元素值到第二个元素值范围并返回1，直至最后一个元素；如果全部不匹配返回<see cref="ICollection{T}.Count"/>
        /// </para>
        /// <para>如果<paramref name="probability"/>小于0则返回-1，</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static int SelectListRange(this IList<double> list, double probability)
        {
            if (list is null) throw new ArgumentNullException();
            var late = 0d;
            int length = list.Count;
            if (probability < 0) return -1;
            int i;
            for (i = 0; i < length; i++)
            {
                var pv = list[i];

                if (late <= probability && probability < pv)
                {
                    return i;
                }

                late = pv;
            }

            return i;
        }

        /// <summary>
        /// 按照概率值挑选有序集合内的归一化值
        /// </summary>
        /// <param name="list">
        /// <para>存放归一化数值的有序集合</para>
        /// <para>该集合内的所有值，全部是范围在[0,1)的浮点值，并且该集合元素的顺序是从小到大排列</para>
        /// <para>如果集合参数不符合要求则不保证返回值符合要求</para>
        /// </param>
        /// <param name="probability">
        /// <para>一个范围区间在[0,1)的浮点值</para>
        /// <para>如果参数大于或等于1则返回值永远是<see cref="ICollection{T}.Count"/>，参数小于0时函数返回-1</para>
        /// </param>
        /// <returns>
        /// <para>集合内匹配顺序范围的元素索引</para>
        /// <para>
        /// 按顺序遍历集合<paramref name="list"/>，判断<paramref name="probability"/>是否处于从0开始到第一个元素的值所在范围并返回0，如果不匹配则继续判断第一个元素值到第二个元素值范围并返回1，直至最后一个元素；如果全部不匹配返回<see cref="ICollection{T}.Count"/>
        /// </para>
        /// <para>如果<paramref name="probability"/>小于0则返回-1，</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static int SelectListRange(this IList<float> list, float probability)
        {
            if (list is null) throw new ArgumentNullException();
            var late = 0f;
            int length = list.Count;
            if (probability < 0) return -1;
            int i;
            for (i = 0; i < length; i++)
            {
                var pv = list[i];

                if (late <= probability && probability < pv)
                {
                    return i;
                }

                late = pv;
            }

            return i;
        }

        /// <summary>
        /// 按照概率值挑选有序集合内的归一化值
        /// </summary>
        /// <param name="list">
        /// <para>存放归一化数值的有序集合</para>
        /// <para>该集合内的所有值，全部是范围在[0,1)的浮点值，并且该集合元素的顺序是从小到大排列</para>
        /// <para>如果集合参数不符合要求则不保证返回值符合要求</para>
        /// </param>
        /// <param name="probability">
        /// <para>一个范围区间在[0,1)的浮点值</para>
        /// <para>如果参数大于或等于1则返回值永远是<see cref="ICollection{T}.Count"/>，参数小于0时函数返回-1</para>
        /// </param>
        /// <returns>
        /// <para>集合内匹配顺序范围的元素索引</para>
        /// <para>
        /// 按顺序遍历集合<paramref name="list"/>，判断<paramref name="probability"/>是否处于从0开始到第一个元素的值所在范围并返回0，如果不匹配则继续判断第一个元素值到第二个元素值范围并返回1，直至最后一个元素；如果全部不匹配返回<see cref="ICollection{T}.Count"/>
        /// </para>
        /// <para>如果<paramref name="probability"/>小于0则返回-1，</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static int SelectListRange(this IReadOnlyList<double> list, double probability)
        {
            if (list is null) throw new ArgumentNullException();
            var late = 0d;
            int length = list.Count;
            if (probability < 0) return -1;
            int i;
            for (i = 0; i < length; i++)
            {
                var pv = list[i];
                if (late <= probability && probability < pv)
                {
                    return i;
                }
                late = pv;
            }

            return i;
        }

        /// <summary>
        /// 按照概率值挑选有序集合内的归一化值
        /// </summary>
        /// <param name="list">
        /// <para>存放归一化数值的有序集合</para>
        /// <para>该集合内的所有值，全部是范围在[0,1)的浮点值，并且该集合元素的顺序是从小到大排列</para>
        /// <para>如果集合参数不符合要求则不保证返回值符合要求</para>
        /// </param>
        /// <param name="probability">
        /// <para>一个范围区间在[0,1)的浮点值</para>
        /// <para>如果参数大于或等于1则返回值永远是<see cref="ICollection{T}.Count"/>，参数小于0时函数返回-1</para>
        /// </param>
        /// <returns>
        /// <para>集合内匹配顺序范围的元素索引</para>
        /// <para>
        /// 按顺序遍历集合<paramref name="list"/>，判断<paramref name="probability"/>是否处于从0开始到第一个元素的值所在范围并返回0，如果不匹配则继续判断第一个元素值到第二个元素值范围并返回1，直至最后一个元素；如果全部不匹配返回<see cref="ICollection{T}.Count"/>
        /// </para>
        /// <para>如果<paramref name="probability"/>小于0则返回-1，</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">参数是null</exception>
        public static int SelectListRange(this IReadOnlyList<float> list, float probability)
        {
            if (list is null) throw new ArgumentNullException();
            var late = 0f;
            int length = list.Count;
            if (probability < 0) return -1;
            int i;
            for (i = 0; i < length; i++)
            {
                var pv = list[i];

                if (late <= probability && probability < pv)
                {
                    return i;
                }

                late = pv;
            }

            return i;
        }

        #endregion

        #region seed

        /// <summary>
        /// 根据本地时间和计算机启动时间返回一个值，可用于随机数生成器的种子
        /// </summary>
        /// <returns>根据本地时间和计算机启动时间生成的值</returns>
        public static long GetSeedByTickXORNowTime()
        {
            return (long)(((ulong)DateTime.Now.Ticks) ^ (((ulong)Environment.TickCount) << 31));
        }

        /// <summary>
        /// 根据UTC时间和计算机启动时间返回一个值，可用于随机数生成器的种子
        /// </summary>
        /// <returns>根据本地时间和计算机启动时间生成的值</returns>
        public static long GetSeedByTickXORUtcNowTime()
        {
            return (long)(((ulong)DateTime.UtcNow.Ticks) ^ (((ulong)Environment.TickCount) << 31));
        }

        #endregion

    }

}
