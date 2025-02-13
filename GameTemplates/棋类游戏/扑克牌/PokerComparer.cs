using Cheng.GameTemplates.Pokers;
using System.Collections.Generic;

namespace Cheng.Algorithm.Sorts.Comparers.Pokers
{

    /// <summary>
    /// 扑克排序--斗地主模式
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
}
