using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cheng.GameTemplates.Pokers
{

    public static class PokerGame
    {

        /// <summary>
        /// 获取一个可获取到一整副扑克牌的迭代器
        /// </summary>
        /// <param name="allowJoker">是否允许有鬼牌</param>
        /// <returns>可连续生成出一整副扑克的迭代器</returns>
        public static IEnumerable<Poker> GetPokerCards(bool allowJoker)
        {
            int i, j;

            for (i = 1; i < 14; i++)
            {
                for (j = 1; j < 5; j++)
                {
                    yield return new Poker((PokerNum)i, (PokerFlower)j);
                }
            }
            if (allowJoker)
            {
                yield return Poker.Kid;
                yield return Poker.Joker;
            }
        }

        /// <summary>
        /// 获取一个可获取到54张扑克牌的迭代器
        /// </summary>
        /// <returns>可连续生成出54张扑克的迭代器</returns>
        public static IEnumerable<Poker> GetPokerCards()
        {
            return GetPokerCards(true);
        }

        /// <summary>
        /// 在指定数组内填入一副扑克牌
        /// </summary>
        /// <param name="pokers">要填入的数组，若容量大于52或53时，填入大小鬼牌</param>
        /// <exception cref="ArgumentNullException">数组为null</exception>
        /// <exception cref="ArgumentException">数组容量小于52</exception>
        public static void GetPokerArray(Poker[] pokers)
        {

            if (pokers is null) throw new ArgumentNullException();
            int length = pokers.Length;

            if (length < 52) throw new ArgumentException();

            int i, j;
            int index = 0;
            for (i = 1; i < 14; i++)
            {
                for (j = 1; j < 5; j++)
                {
                    pokers[index] = new Poker((PokerNum)i, (PokerFlower)j);
                    index++;
                }
            }

            if (length > 53)
            {
                pokers[52] = Poker.Kid;
                if(length > 54) pokers[53] = Poker.Joker;
            }
        }

    }
}
