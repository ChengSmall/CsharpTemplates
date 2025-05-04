using Cheng.GameTemplates.Pokers;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Cheng.Algorithm.Sorts.Comparers.Pokers
{

    /// <summary>
    /// 扑克排序 - 斗地主规则
    /// </summary>
    public sealed class PokerComparerDouDiZhu : Comparer<Poker>
    {

        static int ToNum(PokerNum num)
        {
            if(num >= PokerNum._3 && num <= PokerNum.K)
            {
                return ((int)num) - 3;
            }

            if(num == PokerNum.A || num == PokerNum._2)
            {
                return ((int)num) + 11;
            }

            if (num == PokerNum.LittleJoker) return 100;
            if (num == PokerNum.Joker) return 1000;

            return (int)num;

        }

        public override int Compare(Poker x, Poker y)
        {
            int r = ToNum(x.Num).CompareTo(ToNum(y.Num));

            return r != 0 ? r : ((byte)x.Flower).CompareTo((byte)y.Flower);

        }

    }

    /// <summary>
    /// 扑克组排序 - 21点规则
    /// </summary>
    public sealed class PokerComparer21Point : Comparer<IEnumerable<Poker>>, IComparer<IList<Poker>>, IComparer<Poker[]>
    {

        static int getPoint(PokerNum num)
        {
            //var num = poker.Num;
            if(num > PokerNum._10 && num <= PokerNum.K)
            {
                return 10;
            }
            return (int)num;
        }

        /// <summary>
        /// 计算一组扑克牌的21点值
        /// </summary>
        /// <param name="list">一组扑克</param>
        /// <returns>最终值，按理论最大值计算</returns>
        /// <exception cref="ArgumentNullException">集合是null</exception>
        public static int GetPokersPoint(IEnumerable<Poker> list)
        {
            if (list is null) throw new ArgumentNullException();
            int i;
            //总点数
            int point = 0;
            //A的数量
            int count_A = 0;

            //int count = list.Count;
            //Poker poker;
            PokerNum pn;
            foreach (var item in list)
            {
                //poker = list[i];
                pn = item.Num;
                if (pn == PokerNum.A)
                {
                    count_A++;
                    point += 10; //默认按10点算
                }
                else
                {
                    point += getPoint(pn); //默认点数获取
                }
            }

            //按A牌数量计数 且 点数超出21点
            for(i = count_A; i > 0 && point > 21; i--)
            {
                //将其中一个A牌变为1
                point -= 9;
            }

            return point;
        }

        /// <summary>
        /// 计算一组扑克牌的21点值
        /// </summary>
        /// <param name="array">一组扑克</param>
        /// <returns>最终值，按理论最大值计算；如果集合是null直接返回0</returns>
        public static int GetPokersPoint(params Poker[] array)
        {
            if (array is null) return 0;
            return GetPokersPoint(array as IList<Poker>);
        }

        /// <summary>
        /// 对比两组扑克按照21点规则比较大小
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns><paramref name="x"/>小于<paramref name="y"/>返回值小于0；<paramref name="x"/>大于<paramref name="y"/>返回值大于0；<paramref name="x"/>等于<paramref name="y"/>返回0</returns>
        public override int Compare(IEnumerable<Poker> x, IEnumerable<Poker> y)
        {
            //if (x is null || y is null) throw new ArgumentNullException();
            return GetPokersPoint(x).CompareTo(GetPokersPoint(y));
        }

        int IComparer<Poker[]>.Compare(Poker[] x, Poker[] y)
        {
            return GetPokersPoint(x).CompareTo(GetPokersPoint(y));
        }

        int IComparer<IList<Poker>>.Compare(IList<Poker> x, IList<Poker> y)
        {
            return GetPokersPoint(x).CompareTo(GetPokersPoint(y));
        }
    }

}
