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

        static short ToNum(PokerNum num)
        {
            if (num >= PokerNum._3 && num <= PokerNum.K)
            {
                return (short)(((int)num) - 3);
            }

            if (num == PokerNum.A || num == PokerNum._2)
            {
                return (short)(((int)num) + 11);
            }

            if (num == PokerNum.LittleJoker) return 100;
            if (num == PokerNum.Joker) return 1000;

            return (short)num;

        }

        public override int Compare(Poker x, Poker y)
        {
            int r = ToNum(x.Num) - ToNum(y.Num);
            return r != 0 ? r : (((byte)x.Flower) - ((byte)y.Flower));
        }

    }

    /// <summary>
    /// 扑克组排序 - 21点规则
    /// </summary>
    /// <typeparam name="T">表示一个扑克牌牌组</typeparam>
    public sealed class PokerComparer21Point<T> : IComparer<T> where T : class, IEnumerable<Poker>
    {

        public PokerComparer21Point()
        {
            p_boon = true;
        }

        /// <summary>
        /// 初始化排序器并指定是否实施爆牌机制
        /// </summary>
        /// <param name="boon">如参数为true，则当点数大于21时视为无点数</param>
        public PokerComparer21Point(bool boon)
        {
            p_boon = boon;
        }

        private readonly bool p_boon;

        /// <summary>
        /// 对比两组扑克按照21点规则比较大小
        /// </summary>
        /// <param name="x">前一个牌组</param>
        /// <param name="y">后一个牌组</param>
        /// <param name="boon">是否实施爆牌机制，超出21点的牌视为最低点数，两方皆爆则表示为相等</param>
        /// <returns>按照21点公共规则返回比较的值</returns>
        public static int Comparer(IEnumerable<Poker> x, IEnumerable<Poker> y, bool boon)
        {
            var xv = PokerGameExtends.GetPokersPoint(x);
            var yv = PokerGameExtends.GetPokersPoint(y);
            if (boon)
            {
                if (xv > 21) xv = -1;
                if (yv > 21) yv = -1;
            }
            return xv - yv;
        }

        /// <summary>
        /// 对比两组扑克按照21点规则比较大小
        /// </summary>
        /// <param name="x">前一个值</param>
        /// <param name="y">后一个值</param>
        /// <returns>按照21点公共规则比较牌组，根据点数比较，并实施爆牌机制</returns>
        public static int Comparer(IEnumerable<Poker> x, IEnumerable<Poker> y)
        {
            return Comparer(x, y, true);
        }

        public int Compare(T x, T y)
        {
            return Comparer(x, y, p_boon);
        }

    }

}
