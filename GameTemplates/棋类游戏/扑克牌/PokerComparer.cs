using System.Collections.Generic;

namespace Cheng.GameTemplates.Pokers.Comparers
{

    /// <summary>
    /// 扑克排序--斗地主模式
    /// </summary>
    public class PokerComparerDouDiZhu : Comparer<Poker>
    {

        static int ToNum(PokerNum num)
        {
            if(num >= PokerNum._3 && num <= PokerNum.K)
            {
                return ((int)num) - 3;
            }

            if(num == PokerNum.A || num == PokerNum._2)
            {
                return ((int)num) + 13;
            }

            if (num == PokerNum.Kid) return 100;
            if (num == PokerNum.Joker) return 1000;

            return (int)num;

        }

        public override int Compare(Poker x, Poker y)
        {
            x.GetValue(out var xn, out var xf);
            y.GetValue(out var yn, out var yf);

            int r = ToNum(xn).CompareTo(ToNum(yn));

            if (r != 0) return r;

            return ((byte)xf).CompareTo((byte)yf); ;

        }
    }
}
