using Cheng.Algorithm.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cheng.Algorithm.Randoms.Extends
{

    /// <summary>
    /// 随机器扩展
    /// </summary>
    public static class RandomExtends
    {

        #region 集合

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
            int i, k, j;

            for (k = 0; k < Chaosfrequency; k++)
            {
                //for (i = index; i < length; i++)
                //{
                //    list.Swap(i, random.Next(0, length));
                //}

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

    }
}
