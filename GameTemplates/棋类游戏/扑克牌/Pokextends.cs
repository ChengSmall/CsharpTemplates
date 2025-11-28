using Cheng.Algorithm.Sorts.Comparers.Pokers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.GameTemplates.Pokers
{

    public static class PokerGameExtends
    {

        #region 初始化功能

        /// <summary>
        /// 获取一个可获取到一副或多副扑克牌的迭代器
        /// </summary>
        /// <param name="allowJoker">每一副是否允许有鬼牌</param>
        /// <param name="count">扑克牌副数量</param>
        /// <returns>可连续生成出一副或多副扑克牌的迭代器</returns>
        public static IEnumerable<Poker> GetPokerCards(bool allowJoker, int count)
        {
            int i, j;

            for (int c = 0; c < count; c++)
            {
                for (j = 1; j < 5; j++)
                {
                    for (i = 1; i < ((int)PokerNum.K + 1); i++)
                    {
                        yield return new Poker((PokerNum)i, (PokerFlower)j);
                    }
                }

                if (allowJoker)
                {
                    yield return Poker.LittleJoker;
                    yield return Poker.Joker;
                }
            }
        }

        /// <summary>
        /// 获取一个可获取到一整副扑克牌的迭代器
        /// </summary>
        /// <param name="allowJoker">是否允许有鬼牌</param>
        /// <returns>可连续生成出一整副扑克的迭代器</returns>
        public static IEnumerable<Poker> GetPokerCards(bool allowJoker)
        {
            return GetPokerCards(allowJoker, 1);
        }

        /// <summary>
        /// 获取一个可获取到54张扑克牌的迭代器
        /// </summary>
        /// <returns>可连续生成出54张扑克的迭代器</returns>
        public static IEnumerable<Poker> GetPokerCards()
        {
            return GetPokerCards(true, 1);
        }

        /// <summary>
        /// 在指定数组内填入一副扑克牌
        /// </summary>
        /// <param name="pokers">要填入的数组，若容量大于52或大于53时，填入大小鬼牌</param>
        /// <param name="index">起始索引</param>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        /// <exception cref="ArgumentException">数组可填充容量小于52</exception>
        public static void GetPokerArray(Poker[] pokers, int index)
        {

            if (pokers is null) throw new ArgumentNullException();
            if (index < 0) throw new ArgumentException();
            int length;
            length = pokers.Length - index;

            if (length < 52) throw new ArgumentException();

            int i, j;
            //int index = 0;
            for (j = 1; j < 5; j++)
            {
                for (i = 1; i < ((int)PokerNum.K + 1); i++)
                {
                    pokers[index] = new Poker((PokerNum)i, (PokerFlower)j);
                    index++;
                }
            }

            if (length >= 53)
            {
                pokers[index++] = Poker.LittleJoker;
                if (length >= 54) pokers[index] = Poker.Joker;
            }
        }

        /// <summary>
        /// 在指定数组内填入一副扑克牌
        /// </summary>
        /// <param name="pokers">要填入的数组，若容量大于52或大于53时，填入大小鬼牌</param>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        /// <exception cref="ArgumentException">数组可填充容量小于52</exception>
        public static void GetPokerArray(Poker[] pokers)
        {
            GetPokerArray(pokers, 0);
        }

        #endregion

        #region 21点

        /// <summary>
        /// 计算一组扑克牌的21点值
        /// </summary>
        /// <param name="list">一组扑克</param>
        /// <returns>最终值，按理论最大值计算；null则返回-1</returns>
        public static int GetPokersPoint(IEnumerable<Poker> list)
        {
            if (list is null) return -1;
            int i;
            //总点数
            int point = 0;
            //A的数量
            int count_A = 0;

            bool isEmpty = true;
            PokerNum pn;
            foreach (Poker item in list)
            {
                isEmpty = false;
                pn = item.Num;
                //等于A
                if (pn == PokerNum.A)
                {
                    count_A++;
                    point += 11; //默认按11点算
                }
                else
                {
                    point += getPoint(pn); //获取点数
                }
            }
            if (isEmpty) return -1;

            //按A牌数量计数 且 点数超出21点
            for (i = count_A; i > 0 && point > 21; i--)
            {
                //将其中一个A牌变为1
                point -= 10;
            }

            return point;


            int getPoint(PokerNum t_num)
            {
                //var num = poker.Num;
                if (t_num > PokerNum._10 && t_num <= PokerNum.K)
                {
                    return 10;
                }
                return (int)t_num;
            }
        }

        /// <summary>
        /// 计算一组扑克牌的21点值
        /// </summary>
        /// <param name="array">一组扑克</param>
        /// <returns>最终值，按理论最大值计算；null则返回-1</returns>
        public static int GetPokersPoint(params Poker[] array)
        {
            if (array is null) return -1;
            if (array.Length == 0) return 0;
            int i;
            //总点数
            int point = 0;
            //A的数量
            int count_A = 0;

            PokerNum pn;
            foreach (Poker item in array)
            {
                pn = item.Num;
                //等于A
                if (pn == PokerNum.A)
                {
                    count_A++;
                    point += 11; //默认按11点算
                }
                else
                {
                    point += getPoint(pn); //获取点数
                }
            }

            //按A牌数量计数 且 点数超出21点
            for (i = count_A; i > 0 && point > 21; i--)
            {
                //将其中一个A牌变为1
                point -= 10;
            }

            return point;


            int getPoint(PokerNum t_num)
            {
                //var num = poker.Num;
                if (t_num > PokerNum._10 && t_num <= PokerNum.K)
                {
                    return 10;
                }
                return (int)t_num;
            }
        }

        #endregion

        #region 判断

        /// <summary>
        /// 判断扑克是否属于一张鬼牌
        /// </summary>
        /// <param name="poker"></param>
        /// <returns></returns>
        public static bool IsJoker(Poker poker)
        {
            return poker.IsJoker;
        }

        /// <summary>
        /// 判断扑克是否不属于鬼牌
        /// </summary>
        /// <param name="poker"></param>
        /// <returns></returns>
        public static bool IsNotJoker(Poker poker)
        {
            return poker.Num < PokerNum.LittleJoker;
        }

        #endregion

    }

}
